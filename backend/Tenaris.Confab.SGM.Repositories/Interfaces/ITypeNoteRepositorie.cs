using System.Collections.Generic;
using Tenaris.Confab.SGM.Domain.Entities;

namespace Tenaris.Confab.SGM.Repositories
{
    public interface ITypeNoteRepositorie
    {
        List<TypeNote> GetTypeNotes();
    }
}