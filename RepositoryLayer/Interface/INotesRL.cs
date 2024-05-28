using ModelLayer.Model;
using RepositoryLayer.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface INotesRL
    {
        public UserNotes AddNotes(NotesModel model, int personId);
        public NoteResponseModel ShowNotes(int personId);
        public NoteResponseModel ShowNotesById(int personId, int noteId);
        public UserNotes updateNotes(int personId, NotesModel model, int noteId);
        public  bool Archive_UnArchiveNote(int personId, int notesId);
        public int DeleteNote(int PersonId, int NotesId);
        public bool trash_untrash(int noteId, int personId);
    }
}
