using System;
using System.Collections.Generic;
using System.Linq;

namespace appdev.Services;

public class ReviewItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = "🐱";
    public int Rating { get; set; } = 5;
    public string Category { get; set; } = "General Feedback";
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int LikesCount { get; set; } = 0;
    public bool IsLiked { get; set; } = false;
}

public class ReviewsService
{
    private readonly List<ReviewItem> _reviews = new();

    public event Action? OnChange;

    public ReviewsService()
    {
        // Seed initial reviews to give a lively starting state
        _reviews.AddRange(new[]
        {
            new ReviewItem
            {
                Id = "seed-1",
                UserName = "SakuraNeko",
                UserAvatar = "🐱",
                Rating = 5,
                Category = "Cat Ears + Lasers",
                Comment = "The laser cat ears filter is ridiculously cute!! My camera feed turned into an anime intro instantly. 10/10 sparkle points! ✨✨",
                CreatedAt = DateTime.Now.AddHours(-3),
                LikesCount = 14,
                IsLiked = false
            },
            new ReviewItem
            {
                Id = "seed-2",
                UserName = "MochiBoba",
                UserAvatar = "🐰",
                Rating = 5,
                Category = "Blushing Potato",
                Comment = "Blushing potato mode is pure therapy after a long workday. It runs super smoothly at 60 FPS too! 💕",
                CreatedAt = DateTime.Now.AddDays(-1),
                LikesCount = 9,
                IsLiked = false
            },
            new ReviewItem
            {
                Id = "seed-3",
                UserName = "CyberPixel",
                UserAvatar = "🦊",
                Rating = 4,
                Category = "Performance",
                Comment = "Really great face tracking and zero lag on device. Would love even more filter options like cyberpunk cat glasses!",
                CreatedAt = DateTime.Now.AddDays(-2),
                LikesCount = 6,
                IsLiked = false
            },
            new ReviewItem
            {
                Id = "seed-4",
                UserName = "StarlightKitten",
                UserAvatar = "🦄",
                Rating = 5,
                Category = "Nyan Trail",
                Comment = "The rainbow nyan trail sparkles are so whimsical! Definitely sharing this with all my stream friends. 🌈🐾",
                CreatedAt = DateTime.Now.AddDays(-3),
                LikesCount = 21,
                IsLiked = false
            }
        });
    }

    public IReadOnlyList<ReviewItem> GetAllReviews()
    {
        lock (_reviews)
        {
            return _reviews.ToList();
        }
    }

    public void AddReview(ReviewItem review)
    {
        if (review == null) return;
        lock (_reviews)
        {
            _reviews.Insert(0, review);
        }
        NotifyStateChanged();
    }

    public void ToggleLike(string reviewId)
    {
        lock (_reviews)
        {
            var item = _reviews.FirstOrDefault(r => r.Id == reviewId);
            if (item != null)
            {
                if (item.IsLiked)
                {
                    item.IsLiked = false;
                    item.LikesCount = Math.Max(0, item.LikesCount - 1);
                }
                else
                {
                    item.IsLiked = true;
                    item.LikesCount++;
                }
            }
        }
        NotifyStateChanged();
    }

    public bool DeleteReview(string reviewId)
    {
        bool removed = false;
        lock (_reviews)
        {
            var item = _reviews.FirstOrDefault(r => r.Id == reviewId);
            if (item != null)
            {
                removed = _reviews.Remove(item);
            }
        }
        if (removed)
        {
            NotifyStateChanged();
        }
        return removed;
    }

    public double GetAverageRating()
    {
        lock (_reviews)
        {
            if (_reviews.Count == 0) return 0.0;
            return Math.Round(_reviews.Average(r => r.Rating), 1);
        }
    }

    public int GetRatingCount(int stars)
    {
        lock (_reviews)
        {
            return _reviews.Count(r => r.Rating == stars);
        }
    }

    public int GetTotalCount()
    {
        lock (_reviews)
        {
            return _reviews.Count;
        }
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}
