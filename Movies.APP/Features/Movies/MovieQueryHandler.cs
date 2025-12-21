using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies
{
    public class MovieQueryRequest : Request, IRequest<List<MovieQueryResponse>>
    {
        public string Name { get; set; }
        public DateOnly? ReleaseDateStart { get; set; }
        public DateOnly? ReleaseDateEnd { get; set; }
        public decimal? TotalRevenueStart { get; set; }
        public decimal? TotalRevenueEnd { get; set; }
        public int? DirectorId { get; set; }
        public List<int> GenreIds { get; set; } = new();
    }

    public class MovieQueryResponse : Response
    {
        public string Name { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public decimal? TotaRevenue { get; set; }
        public int? DirectorId { get; set; }
        public List<int> GenreIds { get; set; }

        // formatted / custom fields
        public string ReleaseDateF { get; set; }
        public string TotaRevenueF { get; set; }
        public string Director { get; set; }
        public List<string> Genres { get; set; }
    }

    public class MovieQueryHandler 
        : Service<Movie>, IRequestHandler<MovieQueryRequest, List<MovieQueryResponse>>
    {
        public MovieQueryHandler(DbContext db) : base(db)
        {
        }

        /// <summary>
        /// Base query with all required Includes
        /// </summary>
        protected override IQueryable<Movie> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking)
                .Include(m => m.Director)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .OrderBy(m => m.Name)
                .ThenBy(m => m.ReleaseDate);
        }
        public async Task<List<MovieQueryResponse>> Handle(
            MovieQueryRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Adım: Temel sorguyu ve filtreleri hazırla (Bu aşama IQueryable - Henüz SQL çalışmadı)
            var entityQuery = Query();

            // İsim filtresi
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim();
                entityQuery = entityQuery.Where(m => m.Name.Contains(name));
            }

            // Tarih filtreleri
            if (request.ReleaseDateStart.HasValue)
            {
                var start = request.ReleaseDateStart.Value;
                entityQuery = entityQuery.Where(m => m.ReleaseDate >= start);
            }

            if (request.ReleaseDateEnd.HasValue)
            {
                var end = request.ReleaseDateEnd.Value;
                entityQuery = entityQuery.Where(m => m.ReleaseDate <= end);
            }

            // Gelir filtreleri
            if (request.TotalRevenueStart.HasValue)
            {
                var startRev = request.TotalRevenueStart.Value;
                entityQuery = entityQuery.Where(m => m.TotaRevenue >= startRev);
            }

            if (request.TotalRevenueEnd.HasValue)
            {
                var endRev = request.TotalRevenueEnd.Value;
                entityQuery = entityQuery.Where(m => m.TotaRevenue <= endRev);
            }

            // Yönetmen filtresi
            if (request.DirectorId.HasValue)
            {
                entityQuery = entityQuery.Where(m => m.DirectorId == request.DirectorId.Value);
            }

            // Tür (Genre) filtresi
            if (request.GenreIds is { Count: > 0 })
            {
                entityQuery = entityQuery.Where(m =>
                    m.MovieGenres.Any(mg => request.GenreIds.Contains(mg.GenreId)));
            }

            // 2. Adım: Veriyi DATABASE'den çek (Materialization)
            // ToListAsync çağrıldığı an SQL sorgusu veritabanına gider.
            var moviesFromDb = await entityQuery.ToListAsync(cancellationToken);

            // 3. Adım: BELLEKTE (In-Memory) DTO Dönüşümü ve Formatlama
            // Artık veri RAM'de olduğu için ToString() ve karmaşık C# işlemleri hata vermez.
            var response = moviesFromDb.Select(m => new MovieQueryResponse
            {
                Id = m.Id,
                Guid = m.Guid,
                Name = m.Name,
                ReleaseDate = m.ReleaseDate,
                TotaRevenue = m.TotaRevenue,
                DirectorId = m.DirectorId,

                // Genre ID listesi
                GenreIds = m.MovieGenres?.Select(mg => mg.GenreId).ToList() ?? new List<int>(),

                // Tarih formatlama (Hatanın ana sebeplerinden biri buydu)
                ReleaseDateF = m.ReleaseDate.HasValue
                    ? m.ReleaseDate.Value.ToString("MM/dd/yyyy")
                    : string.Empty,

                // Sayı formatlama
                TotaRevenueF = m.TotaRevenue.HasValue 
                    ? m.TotaRevenue.Value.ToString("N2") 
                    : "0.00",

                // Yönetmen adı birleştirme (Bu da SQL'e çevrilemiyordu)
                Director = m.Director != null
                    ? $"{m.Director.FirstName} {m.Director.LastName}".Trim()
                    : string.Empty,

                // Tür isimleri listesi
                Genres = m.MovieGenres?
                    .Where(mg => mg.Genre != null)
                    .Select(mg => mg.Genre.Name)
                    .ToList() ?? new List<string>()
            }).ToList();

            return response;
        }
    }
}
