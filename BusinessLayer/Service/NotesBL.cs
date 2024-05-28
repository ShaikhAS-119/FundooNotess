using BusinessLayer.Interface;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.Repository.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BusinessLayer.Service
{
    public class NotesBL : INotesBL
    {
        private readonly INotesRL _notesRL;

        public NotesBL(INotesRL notesRL)
        {
            _notesRL = notesRL;
        }
        
        public UserNotes AddNote(NotesModel model, int PersonId)
        {
            
            var data = _notesRL.AddNotes(model, PersonId);
            return data;                         
        }

        public NoteResponseModel ShowNotes(int personId)
        {
            var note = _notesRL.ShowNotes(personId);
            return note;                        
        }
        public NoteResponseModel ShowNotesById(int personId, int noteId)
        {
            var notes =_notesRL.ShowNotesById(personId, noteId);
            return notes;
        }
        public UserNotes updateNotes(int personId, NotesModel model, int noteId)
        {
            return _notesRL.updateNotes(personId, model, noteId);
        }
        public bool Archive_UnArchiveNote(int personId, int notesId)
        {
           var data= _notesRL.Archive_UnArchiveNote(personId, notesId);

            return data;
        }

        public bool DeleteNote(int PersonId, int NotesId)
        {
            var data = _notesRL.DeleteNote(PersonId, NotesId);
            if (data > 0)
            {
                return true;
            }
            return false;
        }

        public bool trash_untrash(int noteId, int personId)
        {
           return  _notesRL.trash_untrash(noteId, personId);
        }
    }
}
