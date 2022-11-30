import {Component, Element, Event, EventEmitter, h, Listen, Method, Prop, State, Watch} from '@stencil/core';
import {debounce} from 'lodash';
import {Container} from "typedi";
import {PanelPosition, PanelStateChangedArgs} from '../../../components/panel/models';
import {
  Activity,
  ActivityDescriptor,
  ActivitySelectedArgs,
  ChildActivitySelectedArgs,
  ContainerSelectedArgs,
  GraphUpdatedArgs
} from '../../../models';
import {ActivityUpdatedArgs} from './activity-properties-editor';
import {ActivityDriverRegistry, ActivityNameFormatter, EventBus, PluginRegistry, PortProviderRegistry} from '../../../services';
import {MonacoEditorSettings} from "../../../services/monaco-editor-settings";
import {ActivityPropertyChangedEventArgs, WorkflowDefinitionPropsUpdatedArgs, WorkflowDefinitionUpdatedArgs, WorkflowEditorEventTypes} from "../models/ui";
import {WorkflowDefinition} from "../models/entities";
import {WorkflowDefinitionsApi} from "../services/api"
import WorkflowDefinitionTunnel, {WorkflowDefinitionState} from "../../../state/workflow-definition-state";

@Component({
  tag: 'elsa-workflow-definition-editor',
  styleUrl: 'editor.scss',
})
export class WorkflowDefinitionEditor {
  @Element() el: HTMLElsaWorkflowDefinitionEditorElement;

  private readonly pluginRegistry: PluginRegistry;
  private readonly eventBus: EventBus;
  private readonly activityNameFormatter: ActivityNameFormatter;
  private readonly portProviderRegistry: PortProviderRegistry;
  private flowchartEditor: HTMLElsaFlowchartEditorElement;
  private container: HTMLDivElement;
  private toolbox: HTMLElsaWorkflowDefinitionEditorToolboxElement;
  private readonly saveChangesDebounced: () => void;
  private readonly workflowDefinitionApi: WorkflowDefinitionsApi;

  constructor() {
    this.eventBus = Container.get(EventBus);
    this.pluginRegistry = Container.get(PluginRegistry);
    this.activityNameFormatter = Container.get(ActivityNameFormatter);
    this.portProviderRegistry = Container.get(PortProviderRegistry);
    this.saveChangesDebounced = debounce(this.saveChanges, 1000);
    this.workflowDefinitionApi = Container.get(WorkflowDefinitionsApi);
  }

  @Prop() workflowDefinition?: WorkflowDefinition;
  @Prop({attribute: 'monaco-lib-path'}) monacoLibPath: string;
  @Event() workflowUpdated: EventEmitter<WorkflowDefinitionUpdatedArgs>
  @State() private workflowDefinitionState: WorkflowDefinition;
  @State() workflowVersions: Array<WorkflowDefinition> = [];

  @Watch('monacoLibPath')
  private handleMonacoLibPath(value: string) {
    const settings = Container.get(MonacoEditorSettings);
    settings.monacoLibPath = value;
  }

  @Watch('workflowDefinition')
  async onWorkflowDefinitionChanged(value: WorkflowDefinition) {
    await this.importWorkflow(value);
  }

  @Listen('graphUpdated')
  private async handleGraphUpdated(e: CustomEvent<GraphUpdatedArgs>) {
    //this.updateModelDebounced();
    this.saveChangesDebounced();
  }

  // @Method()
  // async getCanvas(): Promise<HTMLElsaCanvasElement> {
  //   return this.canvas;
  // }

  @Method()
  async registerActivityDrivers(register: (registry: ActivityDriverRegistry) => void): Promise<void> {
    const registry = Container.get(ActivityDriverRegistry);
    register(registry);
  }

  @Method()
  getWorkflowDefinition(): Promise<WorkflowDefinition> {
    return this.getWorkflowDefinitionInternal();
  }

  @Method()
  async importWorkflow(workflowDefinition: WorkflowDefinition): Promise<void> {
    await this.updateWorkflowDefinition(workflowDefinition);
    //await this.canvas.importGraph(workflowDefinition.root);
    await this.eventBus.emit(WorkflowEditorEventTypes.WorkflowDefinition.Imported, this, {workflowDefinition});
  }

  // Updates the workflow definition without importing it into the designer.
  @Method()
  async updateWorkflowDefinition(workflowDefinition: WorkflowDefinition): Promise<void> {
    this.workflowDefinitionState = workflowDefinition;
  }

  @Method()
  async newWorkflow(): Promise<WorkflowDefinition> {

    const newRoot = await this.flowchartEditor.newRoot();

    const workflowDefinition: WorkflowDefinition = {
      root: newRoot,
      id: '',
      name: 'Workflow 1',
      definitionId: '',
      version: 1,
      isLatest: true,
      isPublished: false,
      materializerName: 'Json'
    }

    await this.updateWorkflowDefinition(workflowDefinition);
    return workflowDefinition;
  }

  @Method()
  async loadWorkflowVersions(): Promise<void> {
    if (this.workflowDefinitionState.definitionId != null && this.workflowDefinitionState.definitionId.length > 0) {
      const workflowVersions = await this.workflowDefinitionApi.getVersions(this.workflowDefinitionState.definitionId);
      this.workflowVersions = workflowVersions.sort(x => x.version).reverse();
    } else {
      this.workflowVersions = [];
    }
  }

  async componentWillLoad() {
    await this.updateWorkflowDefinition(this.workflowDefinition);
    await this.loadWorkflowVersions();
  }

  async componentDidLoad() {
    if (!this.workflowDefinitionState)
      await this.newWorkflow();
    else
      await this.importWorkflow(this.workflowDefinitionState);

    await this.eventBus.emit(WorkflowEditorEventTypes.WorkflowEditor.Ready, this, {workflowEditor: this});
  }

  private getWorkflowDefinitionInternal = async (): Promise<WorkflowDefinition> => {
    const activity: Activity = await this.flowchartEditor.export();
    const workflowDefinition = this.workflowDefinitionState;
    workflowDefinition.root = activity;
    return workflowDefinition;
  };

  private emitActivityChanged = async (activity: Activity, propertyName: string) => {
    await this.eventBus.emit(WorkflowEditorEventTypes.Activity.PropertyChanged, this, activity, propertyName, this);
  };

  private updateModel = async (): Promise<void> => {
    const workflowDefinition = await this.getWorkflowDefinitionInternal();
    await this.updateWorkflowDefinition(workflowDefinition);
  };

  private saveChanges = async (): Promise<void> => {
    const latestVersion = this.workflowVersions.find(v => v.isLatest);
    this.workflowUpdated.emit({workflowDefinition: this.workflowDefinitionState, latestVersionNumber: latestVersion?.version});
  };

  private updateContainerLayout = async (panelClassName: string, panelExpanded: boolean) => {

    if (panelExpanded)
      this.container.classList.remove(panelClassName);
    else
      this.container.classList.toggle(panelClassName, true);

    //await this.updateLayout();
  }

  private onActivityPickerPanelStateChanged = async (e: PanelStateChangedArgs) => await this.updateContainerLayout('activity-picker-closed', e.expanded)
  private onWorkflowEditorPanelStateChanged = async (e: PanelStateChangedArgs) => await this.updateContainerLayout('object-editor-closed', e.expanded)
  private onActivityEditorPanelStateChanged = async (e: PanelStateChangedArgs) => await this.updateContainerLayout('activity-editor-closed', e.expanded)

  private onWorkflowPropsUpdated = (e: CustomEvent<WorkflowDefinitionPropsUpdatedArgs>) => {
    //this.updateModelDebounced();
    this.saveChangesDebounced();
  }

  onVersionSelected = async (e: CustomEvent<WorkflowDefinition>) => {
    const workflowToView = e.detail;
    const workflowDefinition = await this.workflowDefinitionApi.get({definitionId: workflowToView.definitionId, versionOptions: {version: workflowToView.version}});
    await this.importWorkflow(workflowDefinition);
  };

  onDeleteVersionClicked = async (e: CustomEvent<WorkflowDefinition>) => {
    const workflowToDelete = e.detail;
    await this.workflowDefinitionApi.deleteVersion({definitionId: workflowToDelete.definitionId, version: workflowToDelete.version});
    const latestWorkflowDefinition = await this.workflowDefinitionApi.get({definitionId: workflowToDelete.definitionId, versionOptions: {isLatest: true}});
    await this.loadWorkflowVersions();
    await this.importWorkflow(latestWorkflowDefinition);
  };

  onRevertVersionClicked = async (e: CustomEvent<WorkflowDefinition>) => {
    const workflowToRevert = e.detail;
    await this.workflowDefinitionApi.revertVersion({definitionId: workflowToRevert.definitionId, version: workflowToRevert.version});
    const workflowDefinition = await this.workflowDefinitionApi.get({definitionId: workflowToRevert.definitionId, versionOptions: {isLatest: true}});
    await this.loadWorkflowVersions();
    await this.importWorkflow(workflowDefinition);
  };

  render() {
    const state: WorkflowDefinitionState = {
      workflowDefinition: this.workflowDefinition
    };

    return (
      <WorkflowDefinitionTunnel.Provider state={state}>
        <div class="absolute inset-0" ref={el => this.container = el}>
          <elsa-flowchart-editor />
          <elsa-panel
            class="elsa-workflow-editor-container z-30"
            position={PanelPosition.Right}
            onExpandedStateChanged={e => this.onWorkflowEditorPanelStateChanged(e.detail)}>
            <div class="object-editor-container">
              <elsa-workflow-definition-properties-editor
                workflowDefinition={this.workflowDefinitionState}
                workflowVersions={this.workflowVersions}
                onWorkflowPropsUpdated={e => this.onWorkflowPropsUpdated(e)}
                onVersionSelected={e => this.onVersionSelected(e)}
                onDeleteVersionClicked={e => this.onDeleteVersionClicked(e)}
                onRevertVersionClicked={e => this.onRevertVersionClicked(e)}
              />
            </div>
          </elsa-panel>
        </div>
      </WorkflowDefinitionTunnel.Provider>
    );
  }
}
