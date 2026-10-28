
using WordleClone.API.Data;
using WordleClone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace WordleClone.API.Services;

public interface IWordlService
{
    public Task<List<Wordl>> GetAllWordl();
    public Task<Wordl?> GetWordl(int id);
    public Task<bool> AddWordl(Wordl? newWordl);
    public Task<bool> RemoveWordl(int id);
}

public sealed class WordlService : IWordlService
{
    private readonly AppDbContext _context;
    public WordlService(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<bool> AddWordl(Wordl? newWordl)
    {
        if (newWordl == null)
        {
            throw new Exception("Wordl is null");
        }
        if (newWordl.Word.Length != 5)
        {
            throw new Exception("Wordls cant be more or less than 5 letters");
        }
        newWordl.Word = newWordl.Word.ToLower();
        Wordl? repeatingWord = await _context.wordls.FirstOrDefaultAsync(x => x.Word == newWordl.Word);
        if (repeatingWord == null)
        {
            await _context.AddAsync(newWordl);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new Exception("This Wordl is already in the database");
        }
    }

    public async Task<List<Wordl>> GetAllWordl()
    {
        return await _context.wordls.ToListAsync();
    }

    public async Task<Wordl?> GetWordl(int id)
    {
        // var test = _context.wordls.FirstOrDefault(); we can still use non async methods inside a async method !! but no the other way around !!
        return await _context.wordls.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> RemoveWordl(int id)
    {
        Wordl? wordlToRemove = await _context.wordls.FirstOrDefaultAsync(x => x.Id == id);
        if (wordlToRemove != null)
        {
            _context.wordls.Remove(wordlToRemove);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new Exception($"Wordl with id: {id} doesnt exist");
        }
    }
}