using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenaris.Confab.SGM.Domain.Entities;
using Tenaris.Confab.SGM.Repositories;

namespace Tenaris.Confab.SGM.Services
{
    public class NoteFailureService : INoteFailureService
    {

        private INoteFailureRepositorie _NoteFailureRepo;

        public NoteFailureService(INoteFailureRepositorie NoteFailureRepo)
        {
            _NoteFailureRepo = NoteFailureRepo;
        }

        public NoteFailureService()
        {
            _NoteFailureRepo = new NoteFailureRepositorie();
        }

        public List<NoteFailure> GetNotesFailure(DateTime dateStart, DateTime dateEnd, string location)
        {
            return _NoteFailureRepo.GetNotesFailure(dateStart, dateEnd, location);
        }

        
    }
}
