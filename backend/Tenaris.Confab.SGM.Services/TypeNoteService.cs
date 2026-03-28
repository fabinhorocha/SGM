using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class TypeNoteService: ITypeNoteService
    {
        ITypeNoteRepositorie _TypeNoteRepo;

        public TypeNoteService(ITypeNoteRepositorie TypeNoteRepo)
        {

            _TypeNoteRepo = TypeNoteRepo;

        }
        public TypeNoteService()
        {

            _TypeNoteRepo = new TypeNoteRepositorie();

        }


        public List<TypeNote> GetTypeNotes()
        {
            return _TypeNoteRepo.GetTypeNotes();
        } 


    }
}
