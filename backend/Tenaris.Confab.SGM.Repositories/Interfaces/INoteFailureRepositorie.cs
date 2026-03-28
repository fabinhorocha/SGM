using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface INoteFailureRepositorie
    {
        List<NoteFailure> GetNotesFailure(DateTime dateStart, DateTime dateEnd, string location);
       

    }
}