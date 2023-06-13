﻿using Moonglade.Caching;
using Moonglade.Data.Spec;

namespace Moonglade.Core.PostFeature;

public record PurgeRecycledCommand : IRequest;

public class PurgeRecycledCommandHandler : IRequestHandler<PurgeRecycledCommand>
{
    private readonly IBlogCache _cache;
    private readonly IRepository<PostEntity> _repo;

    public PurgeRecycledCommandHandler(IBlogCache cache, IRepository<PostEntity> repo)
    {
        _cache = cache;
        _repo = repo;
    }

    public async Task Handle(PurgeRecycledCommand request, CancellationToken ct)
    {
        var spec = new PostSpec(true);
        var posts = await _repo.ListAsync(spec);
        await _repo.DeleteAsync(posts, ct);

        foreach (var guid in posts.Select(p => p.Id))
        {
            _cache.Remove(CachePartition.Post, guid.ToString());
        }
    }
}