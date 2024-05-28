using BusinessLayer.Interface;
using LoginRegisterAPI.RabbitMQService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.Model;
using RepositoryLayer.Repository.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace LoginRegisterAPI.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesBL _notesBL;

        private readonly IDistributedCache _cache;

        private readonly MessagePublish _messagePublish;

        public NotesController(INotesBL notesBL, IDistributedCache distributedCache, MessagePublish messagePublish)
        {
            _notesBL = notesBL;
            _cache = distributedCache;
            _messagePublish = messagePublish;

        }
        /// <summary>
        /// Create a new Note
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ResponseModel<NotesModel> Createnotes(NotesModel model)
        {

            var response = new ResponseModel<NotesModel>();
            string _personId = User.FindFirstValue("Id");
            int personId = Convert.ToInt32(_personId);

            var data = _notesBL.AddNote(model, personId);

            //checking key presence
            var cacheKey = _cache.GetString(Convert.ToString(data.NoteId));

            //to get by noteId
            if (cacheKey == null && data != null)
            {
                _cache.SetString(Convert.ToString(data.NoteId), JsonSerializer.Serialize(data));
            }

            var cachePersonData = _cache.GetString(_personId);

            //list to get all notes
            if (cachePersonData == null)
            {
                List<UserNotes> noteList = new List<UserNotes> { data };
                _cache.SetString(_personId, JsonSerializer.Serialize(noteList));
            }
            else
            {
                //forst deserialize the list to add value if key is present
                var note = JsonSerializer.Deserialize<List<UserNotes>>(cachePersonData);
                note.Add(data);
                _cache.SetString(_personId, JsonSerializer.Serialize(note));
            }

            if (data != null)
            {
                _messagePublish.sendMessage("Note created successfully");
                response.Success = true;
                response.Message = "Note created successfully";
                response.Data = model;

            }
            else
            {
                response.Success = false;
                response.Message = "failed!! ";

            }
            return response;
        }

        /// <summary>
        /// View all notes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ResponseModel<List<UserNotes>> ViewNotes()
        {

            var _personId = User.FindFirstValue("Id");
            int personId = Convert.ToInt32(_personId);
            //data recived in list from db
            var list1 = _notesBL.ShowNotes(personId);

            var response = new ResponseModel<List<UserNotes>>();

            //data received in cache from redis
            var cacheResponse = _cache.GetString(Convert.ToString(_personId));

            if (cacheResponse != null)
            {
                var cacheNotes = JsonSerializer.Deserialize<List<UserNotes>>(cacheResponse);
                response.Success = true;
                response.Message = "Successfull";
                response.Data = cacheNotes;
            }
            else
            {
                response.Success = false;
                response.Message = "error or data not found";

            }
            return response;

            //need to change response type to NoteResponseModel of list1 type
            /* else
             {
                 if (cacheResponse != null)
                 {

                     response.Success = true;
                     response.Message = "Successfull";
                     response.Data = response2;
                 }
                 else
                 {
                     response.Success = false;
                     response.Message = "UnSuccessfull";
                     response.Data = list1;
                 }

                 return response;
             }*/

        }

        /// <summary>
        /// View Note by ID
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("id")]
        [Authorize]
        public ResponseModel<UserNotes> ViewNotesById(ViewNotesByIdModel model)
        {
            string _personId = User.FindFirstValue("Id");
            int personId = Convert.ToInt32(_personId);
            //data from DB
            var data = _notesBL.ShowNotesById(personId, model.notesId);

            var response = new ResponseModel<UserNotes>();

            //getting redis data by noteid
            var cache = _cache.GetString(Convert.ToString(model.notesId));

            if (cache != null)
            {
                _messagePublish.sendMessage("note by ID Data gotten");
                var cacheResponse = JsonSerializer.Deserialize<UserNotes>(cache);
                response.Success = true;
                response.Message = "successfull";
                response.Data = cacheResponse;
            }
            else
            {
                response.Success = false;
                response.Message = "error or data not found";
            }
            return response;

           
        }

        /// <summary>
        /// Edit notes
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{noteId}")]
        [Authorize]
        public ResponseModel<UserNotes> EditNotes(int noteId, NotesModel model)
        {
            string _personId = User.FindFirstValue("Id");
            int personId = Convert.ToInt32(_personId);

            var data = _notesBL.updateNotes(personId, model, noteId);

            var response = new ResponseModel<UserNotes>();

            if (data != null)
            {
                _messagePublish.sendMessage("Note Edited successfully");
                response.Success = true;
                response.Message = "updated successfully";
                response.Data = data;
            }
            else
            {
                response.Success = false;
                response.Message = "failed to update";
            }

            return response;
        }

        /// <summary>
        /// Archive_UnArchive operation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch()]
        [Authorize]
        public ResponseModel<bool> Archived_Unarchived(Archive_UnArchiveModel model)
        {
            string _personId = User.FindFirstValue("Id");
            int personId = Convert.ToInt32(_personId);

            var data = _notesBL.Archive_UnArchiveNote(personId, model.noteId);

            var response = new ResponseModel<bool>();
            if (data)
            {
                _messagePublish.sendMessage("operation successfull");
                response.Success = true;
                response.Message = "Archive operation success";
                response.Data = data;
            }
            else
            {
                response.Success = false;
                response.Message = "failes";
            }
            return response;
        }

        /// <summary>
        /// Delete note
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public IActionResult Delete(DeleteNoteByIDModel model)
        {
            string _personId = User.FindFirstValue("Id");
            int personId = Convert.ToInt32(_personId);

            var data = _notesBL.DeleteNote(personId, model.delete);
            var response = new ResponseModel<bool>();

            if (data)
            {
                _messagePublish.sendMessage("Note deleted successfully");
                response.Success = true;
                response.Message = "Note deleted successfully";
                response.Data = data;
                return Ok(response);
            }
            else
            {
                response.Success = false;
                response.Message = "failed!! ";
                response.Data = data;

                return BadRequest(response);
            }

        }

        /// <summary>
        /// Trash and UnTrash Operation
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("trash")]
        [Authorize]
        public ResponseModel<bool> Trash_UnTrash(int noteId)
        {
            string _personId = User.FindFirstValue("Id");
            int personId = Convert.ToInt32(_personId);

            var data = _notesBL.trash_untrash(noteId, personId);

            var response = new ResponseModel<bool>();

            if (data)
            {
                _messagePublish.sendMessage("operation successfull");
                response.Success = true;
                response.Message = "successful";
                response.Data = data;
            }
            else
            {
                response.Success = false;
                response.Message = "failed";
            }
            return response;
        }


    }
}
