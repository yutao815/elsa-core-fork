using Elsa.ProtoActor.Extensions;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Models;
using ProtoBookmark = Elsa.ProtoActor.Protos.Bookmark;

namespace Elsa.ProtoActor.Mappers;

/// <summary>
/// Maps between <see cref="Bookmark"/> and <see cref="ProtoBookmark"/>.
/// </summary>
internal class BookmarkMapper
{
    private readonly IBookmarkPayloadSerializer _bookmarkPayloadSerializer;

    public BookmarkMapper(IBookmarkPayloadSerializer bookmarkPayloadSerializer)
    {
        _bookmarkPayloadSerializer = bookmarkPayloadSerializer;
    }

    public Bookmark Map(ProtoBookmark bookmark) => new(bookmark.Id, bookmark.Name, bookmark.Hash, bookmark.Payload, bookmark.ActivityNodeId, bookmark.ActivityInstanceId, bookmark.AutoBurn, bookmark.CallbackMethodName);

    public IEnumerable<ProtoBookmark> Map(IEnumerable<Bookmark> source)
    {
        return source.Select(x =>
        {
            var payload = x.Payload != null ? _bookmarkPayloadSerializer.Serialize<object>(x.Payload) : string.Empty;
            
            return new ProtoBookmark
            {
                Id = x.Id,
                Name = x.Name,
                Hash = x.Hash,
                Payload = payload,
                ActivityNodeId = x.ActivityNodeId,
                ActivityInstanceId = x.ActivityInstanceId,
                AutoBurn = x.AutoBurn,
                CallbackMethodName = x.CallbackMethodName.EmptyIfNull()
            };
        });
    }

    public IEnumerable<Bookmark> Map(IEnumerable<ProtoBookmark> source)
    {
        return source.Select(x =>
        {
            var payload = !string.IsNullOrEmpty(x.Payload) ? _bookmarkPayloadSerializer.Deserialize(x.Payload) : null;

            return new Bookmark(
                x.Id,
                x.Name,
                x.Hash,
                payload,
                x.ActivityNodeId,
                x.ActivityInstanceId,
                x.AutoBurn,
                x.CallbackMethodName.NullIfEmpty());
        });
    }
}