using ModelLayer.Model;
using RepositoryLayer.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interface
{
    public  interface INotesBL
    {
        public UserNotes AddNote(NotesModel model, int PersonId);

        public NoteResponseModel ShowNotes(int personId);

        public NoteResponseModel ShowNotesById(int personId, int noteId);

        public UserNotes updateNotes(int personId, NotesModel model, int noteId);

        public bool Archive_UnArchiveNote(int PersonId, int NotesId);

        public bool DeleteNote(int PersonId, int NotesId);

        public bool trash_untrash(int noteId, int personId);

    }
}
