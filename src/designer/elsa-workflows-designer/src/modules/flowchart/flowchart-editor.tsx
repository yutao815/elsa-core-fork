import {Component, h, Method, Prop, State} from "@stencil/core";
import {PanelPosition, PanelStateChangedArgs} from "../../components/panel/models";
import {Activity, ActivityDescriptor} from "../../models";
import {ActivityUpdatedArgs} from "../workflow-definitions/components/activity-properties-editor";
import {Flowchart} from "./models";

@Component({
  tag: 'elsa-flowchart-editor',
  styleUrl: 'flowchart-editor'
})
export class FlowchartEditor {
  private container: HTMLDivElement;
  private flowchart: HTMLElsaFlowchartElement;
  private toolbox: HTMLElsaWorkflowDefinitionEditorToolboxElement;

  @Prop({attribute: 'monaco-lib-path'}) monacoLibPath: string;
  @State() private selectedActivity?: Activity;

  @Method()
  async newRoot(): Promise<Activity> {
    return await this.flowchart.newRoot();
  }

  @Method()
  async export(): Promise<Activity> {
    return await this.flowchart.export();
  }

  render() {
    return <div class="flowchart-editor-root absolute inset-0">
      <div class="" ref={el => this.container = el}>
        <elsa-flowchart-editor-toolbar zoomToFit={this.onZoomToFit} autoLayout={this.onAutoLayout}/>
        <elsa-panel
          class="elsa-activity-picker-container z-30"
          position={PanelPosition.Left}
          onExpandedStateChanged={e => this.onActivityPickerPanelStateChanged(e.detail)}>
          <elsa-workflow-definition-editor-toolbox ref={el => this.toolbox = el}/>
        </elsa-panel>
        <div>
          <elsa-flowchart
            ref={el => this.flowchart = el}
            interactiveMode={true}
            class="absolute left-0 top-0 right-0 bottom-0"
            onDragOver={this.onDragOver}
            onDrop={this.onDrop}/>
        </div>
        <elsa-panel
          class="elsa-activity-editor-container"
          position={PanelPosition.Bottom}
          onExpandedStateChanged={e => this.onActivityEditorPanelStateChanged(e.detail)}>
          <div class="activity-editor-container">
            {this.renderSelectedObject()}
          </div>
        </elsa-panel>
      </div>
    </div>
  }

  private onZoomToFit = async () => await this.flowchart.zoomToFit();

  private onAutoLayout = async (direction: "TB" | "BT" | "LR" | "RL") => await this.flowchart.autoLayout(direction);

  private onDragOver = (e: DragEvent) => {
    e.preventDefault();
  };

  private onDrop = async (e: DragEvent) => {
    const json = e.dataTransfer.getData('activity-descriptor');
    const activityDescriptor: ActivityDescriptor = JSON.parse(json);

    await this.flowchart.addActivity({
      descriptor: activityDescriptor,
      x: e.pageX,
      y: e.pageY
    });
  };

  private renderSelectedObject = () => {
    if (!!this.selectedActivity)
      return <elsa-activity-properties-editor
        activity={this.selectedActivity}
        //variables={this.workflowDefinitionState.variables}
        onActivityUpdated={e => this.onActivityUpdated(e)}/>;
  }

  private onActivityUpdated = async (e: CustomEvent<ActivityUpdatedArgs>) => {
    await this.flowchart.updateActivity({
      id: e.detail.newId,
      originalId: e.detail.originalId,
      activity: e.detail.activity
    });

    // await this.updateModel();
    // this.emitActivityChangedDebounced({...e.detail, workflowEditor: this.el});
    // this.saveChangesDebounced();
  }

  private updateLayout = async () => {
    await this.flowchart.updateLayout();
  };

  private updateContainerLayout = async (panelClassName: string, panelExpanded: boolean) => {

    if (panelExpanded)
      this.container.classList.remove(panelClassName);
    else
      this.container.classList.toggle(panelClassName, true);

    await this.updateLayout();
  }

  private onActivityPickerPanelStateChanged = async (e: PanelStateChangedArgs) => await this.updateContainerLayout('activity-picker-closed', e.expanded)
  private onWorkflowEditorPanelStateChanged = async (e: PanelStateChangedArgs) => await this.updateContainerLayout('object-editor-closed', e.expanded)
  private onActivityEditorPanelStateChanged = async (e: PanelStateChangedArgs) => await this.updateContainerLayout('activity-editor-closed', e.expanded)
}
