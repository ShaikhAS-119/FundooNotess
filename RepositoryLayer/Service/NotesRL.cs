using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.Repository;
using RepositoryLayer.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;

namespace RepositoryLayer.Service
{
    public class NotesRL : INotesRL
    {
        private readonly scaffoldingDbContext _contextC;
        private readonly IDistributedCache _cache;
        private static ILogger<NotesRL> _logger;
        public NotesRL(scaffoldingDbContext contextC, IDistributedCache distributedCache, ILogger<NotesRL> logger)
        {
            _contextC = contextC;
            _cache = distributedCache;
            _logger = logger;
        }
        public UserNotes AddNotes(NotesModel model, int PersonId)
        {
            UserNotes userNotes = null;

            _logger.LogInformation("AddNotes Api that return usernotes after adding notes");
            try
            {
                userNotes = new UserNotes();
                userNotes.Title = model.Title;
                userNotes.Description = model.Description;
                userNotes.Color = model.Colour;
                userNotes.PersonId = PersonId;

                _contextC.Notes.Add(userNotes);               
                _contextC.SaveChanges();
                
            }
            catch (Exception e)
            {
                _logger.LogError("error:", e);
            }
            return userNotes;
        }

        public NoteResponseModel ShowNotes(int personId)
        {
            List<UserNotes> noteTable = _contextC.Notes.ToList<UserNotes>();

            var data = _contextC.Notes.FirstOrDefault(c => c.PersonId == personId);
            NoteResponseModel notes = new NoteResponseModel();

            foreach (var item in noteTable)
            {
                notes.NoteID = data.NoteId;
                notes.Title = data.Title;
                notes.Description = data.Description;
                notes.Color = data.Color;
            }
            return notes;
        }

        public NoteResponseModel ShowNotesById(int personId, int noteId)
        {
            NoteResponseModel notes = new NoteResponseModel();
            _logger.LogInformation("showNotesByID Api that return notesResponseModel and return notes by id");
            try
            {
                var data = _contextC.Notes.FirstOrDefault(c => c.NoteId == noteId && c.PersonId == personId);

                if (data != null)
                {
                    notes.NoteID = data.NoteId;
                    notes.Title = data.Title;
                    notes.Description = data.Description;
                    notes.Color = data.Color;
                }
            }
            catch (Exception e)
            {

                _logger.LogError("error:", e);
            }
            return notes;
        }
        
        public UserNotes updateNotes(int personId, NotesModel model, int noteId)
        {
            var row = _contextC.Notes.FirstOrDefault(c => c.NoteId == noteId && c.PersonId == personId);
            if (row != null)
            {
                row.Title = model.Title;
                row.Description = model.Description;
                row.Color = model.Colour;
                
                //save to DB
                _contextC.SaveChanges();
                
                //adding to cache if there it will overrdidde
                _cache.SetString(Convert.ToString(row.NoteId), JsonSerializer.Serialize(row));

                //also add to redis list that store all notes
                //geting the person id key bcoz list key is person id
                var cachePersonData = _cache.GetString(Convert.ToString(row.PersonId));
                //deserialize list 
                var note = JsonSerializer.Deserialize<List<UserNotes>>(cachePersonData);
                //find,update, save
                var listRow = note.Find(i => i.NoteId == noteId);
                note.Remove(listRow);
                note.Add(row);
                _cache.SetString(Convert.ToString(row.PersonId), JsonSerializer.Serialize(note));
            }
            return row;

        }
        public bool Archive_UnArchiveNote(int personId, int notesId)
        {
            var data = _contextC.Notes.FirstOrDefault(c => c.NoteId == notesId && c.PersonId == personId);

            if (data.IsDeleted != true)
            {
                if (data.IsArchive == true)
                {
                    data.IsArchive = false;
                    _contextC.SaveChanges();
                }
                else
                {
                    data.IsArchive = true;
                    _contextC.SaveChanges();
                }
                //this true say about the status of archive
                return true;
            }
            return false;
        }

        public int DeleteNote(int PersonId, int NotesId)
        {
            int data = 0;
            _logger.LogInformation("deleteNotes Api that return int ");
            try
            {

                var row = _contextC.Notes.FirstOrDefault(c => c.NoteId == NotesId && c.PersonId == PersonId);

                var cache = _cache.GetString(Convert.ToString(row.PersonId));
                if (cache != null)
                {
                    _cache.Remove(cache);
                }

                if (row != null)
                {
                    _contextC.Notes.Remove(row);
                    data = _contextC.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("error:", e);

            }
            return data;
        }

        public bool trash_untrash(int noteId, int personId)
        {
            _logger.LogInformation("trash_untrash Api that return bool ");
            try
            {
                var rowData = _contextC.Notes.FirstOrDefault(c => c.NoteId == noteId && c.PersonId == personId);
                if (rowData.IsDeleted == true)
                {
                    rowData.IsDeleted = false;
                }
                else
                {
                    rowData.IsDeleted = true;
                }
                _contextC.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError("error:", e);
            }
            return true;
        }
    }
}
