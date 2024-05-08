using Microsoft.EntityFrameworkCore;
using Mvc.Data;
using Mvc.Models.Entity;

namespace Mvc.Service.Impl
{
    public class EnvioService : IEnvioService
    {
        private readonly DatabaseContext _context;

        public EnvioService(DatabaseContext context)
        {
            _context = context;
        }
        
        
        public void Add(Envio envio)
        {
            _context.Envios.Add(envio);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var envioDelete = GetById(id);
            if (envioDelete != null)
            {
                _context.Envios.Remove(envioDelete);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Envio> GetAll()
        {
            return _context.Envios
            .Include(e => e.Usuario)
            .ToList();
        }

        public Envio GetById(int id)
        {
            return _context.Envios.Find(id)!;
        }

        public Envio GetByIdAndUsuarioId(int id, string usuarioId)
        {
            return _context.Envios.FirstOrDefault(e => e.Id == id && e.UsuarioId == usuarioId);
        }

        public async Task<List<Envio>> GetEnviosAsync()
        {
            return await _context.Envios.Include(ce => ce.Usuario).ToListAsync();
        }

        public void Update(Envio envio)
        {
           _context.Envios.Update(envio);
            _context.SaveChanges();
        }
        
    }
}