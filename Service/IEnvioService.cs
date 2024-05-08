using Mvc.Models.Entity;

namespace Mvc.Service
{
    public interface IEnvioService
    {
        IEnumerable<Envio> GetAll();
        Envio GetById(int id);
        void Add(Envio calendarEvent);
        void Update(Envio calendarEvent);
        Envio GetByIdAndUsuarioId(int id, string usuarioId);
        void Delete(int id);
        Task<List<Envio>> GetEnviosAsync();
        
    }
}