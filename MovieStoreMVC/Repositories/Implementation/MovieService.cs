using MovieStoreMVC.Models.DTO;
using MovieStoreMVC.Models.Domain;
using MovieStoreMVC.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly DatabaseContext _databaseContext;

        public MovieService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool Add(Movie model)
        {
            try
            {
                _databaseContext.Movies.Add(model);
                _databaseContext.SaveChanges();

                foreach(var genreId in model.Genres) 
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };
                    _databaseContext.MoviesGenres.Add(movieGenre);
                }

                _databaseContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                    return false;
                var movieGenres = _databaseContext.MoviesGenres.Where(a => a.MovieId == data.Id);
                foreach (var movieGenre in movieGenres)
                {
                    _databaseContext.MoviesGenres.Remove(movieGenre);
                }
                _databaseContext.Movies.Remove(data);
                _databaseContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Movie GetById(int id)
        {
            return _databaseContext.Movies.Find(id);
        }

        public MovieListVm List(string term = "", bool paging = false, int currentPage = 0)
        {
            var data = new MovieListVm();

            var list = _databaseContext.Movies.ToList();


            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                list = list.Where(a => a.Title.ToLower().StartsWith(term)).ToList();
            }

            if (paging)
            {
                // here we will apply paging
                int pageSize = 5;
                int count = list.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                data.PageSize = pageSize;
                data.CurrentPage = currentPage;
                data.TotalPages = TotalPages;
            }

            foreach(var movie in list)
            {
                var genres = (from genre in _databaseContext.Genres
                              join mg in _databaseContext.MoviesGenres
                              on genre.Id equals mg.GenreId
                              where mg.MovieId == movie.Id
                              select genre.GenreName
                              ).ToList();
                var genreNames = string.Join(',', genres);
                movie.GenreNames = genreNames;
            }

            data.MovieList = list.AsQueryable();

            return data;
        }

        public bool Update(Movie model)
        {
            try
            {
                // these genreIds are not selected by users and still present is movieGenre table corresponding to
                // this movieId. So these ids should be removed.
                var genresToDeleted = _databaseContext.MoviesGenres.Where(a => a.MovieId == model.Id && !model.Genres.Contains(a.GenreId)).ToList();
                foreach (var mGenre in genresToDeleted)
                {
                    _databaseContext.MoviesGenres.Remove(mGenre);
                }
                foreach (int genId in model.Genres)
                {
                    var movieGenre = _databaseContext.MoviesGenres.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genId);
                    if (movieGenre == null)
                    {
                        movieGenre = new MovieGenre { GenreId = genId, MovieId = model.Id };
                        _databaseContext.MoviesGenres.Add(movieGenre);
                    }
                }

                _databaseContext.Movies.Update(model);
                // we have to add these genre ids in movieGenre table
                _databaseContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<int> GetGenreByMovieId(int movieId)
        {
            var genreIds = _databaseContext.MoviesGenres.Where(a => a.MovieId == movieId).Select(a => a.GenreId).ToList();
            return genreIds;
        }
    }
}