using MovieStoreMVC.Repositories.Abstract;
using MovieStoreMVC.Models.Domain;

namespace MovieStoreMVC.Repositories.Implementation
{
    public class GenreService : IGenreService
    {
        private readonly DatabaseContext _databaseContext;

        public GenreService(DatabaseContext databaseContext) 
        {
            _databaseContext = databaseContext;
        }

        public bool Add(Genre model)
        {
            try
            {
                _databaseContext.Genres.Add(model);
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
                if (data == null) { return false; }

                _databaseContext.Genres.Remove(data);
                _databaseContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Genre GetById(int id)
        {
            return _databaseContext.Genres.Find(id);
        }

        public IQueryable<Genre> List()
        {
            var data = _databaseContext.Genres.AsQueryable();
            return data;
        }

        public bool Update(Genre model)
        {
            try
            {
                _databaseContext.Genres.Update(model);
                _databaseContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
