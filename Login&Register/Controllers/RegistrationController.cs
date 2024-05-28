
using LoginRegisterAPI.RabbitMQService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace LoginRegisterAPI.Controllers
{
    
    [Route("api/users")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegisterationBL _registerationBL;

        private readonly MessagePublish _messagePublish;
        public  RegistrationController(IRegisterationBL registeration, MessagePublish messagePublish) 
        {
            _registerationBL = registeration;
            _messagePublish = messagePublish;
        }

        /// <summary>
        /// Register new User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register(RegistrationModel model)
        {
            var data = _registerationBL.Register(model);

            var response = new ResponseModel<RegistrationResponse>();

            if (data != null)
            {
                _messagePublish.sendMessage("successfully Registerd");
                response.Success = true;
                response.Message = "Registered Successfully";
                response.Data = data;
                return Ok(response);
            }
            else
            {
                response.Success = false;
                response.Message = "Registered failed";
                response.Data = null;

                return BadRequest(response);
            }

        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public IActionResult LoginUser(LoginModel model)
        {
            var modelData = _registerationBL.Login(model);

            var response = new ResponseModel<string>();
            if (modelData != null)
            {
                _messagePublish.sendMessage("login Successfull");
                response.Success = true;
                response.Message = "Login Successfull";
                response.Data = modelData;

                return Ok(response);
            }
            else
            {

                response.Success = false;
                response.Message = "login failed";
                response.Data = null;              

                return Unauthorized(response);
            }            
        }

        /// <summary>
        /// Forget password 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("forget")]
        public IActionResult ForgetPass(ForgetPasswordModel model)
        {
            var data = _registerationBL.ForgetPassword(model);

            var response = new ResponseModel<bool>();
            if (data)
            {
                _messagePublish.sendMessage("reset via mail, Mail sent Successfull");
                response.Success = true;
                response.Message = "mail sent successfully";
                response.Data = data;
                return Ok(response);
            }
            else
            {
                response.Success = false;
                response.Message = "mail sent error response";
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="token"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset")]
        public IActionResult ResetPass(string token, ResetPasswordModel model)
        {
            var data = _registerationBL.ResetPassword(token, model);

            var response = new ResponseModel<ResetPasswordModel>();

            if (data > 0)
            {
                _messagePublish.sendMessage("reset successfull");
                response.Success = true;
                response.Message = "password reset successfully";
                response.Data = null;
                return Ok(response);
            }
            else
            {
                response.Success = false;
                response.Message = "Registered failed";
                response.Data = null;

                return BadRequest(response);
            }
        }
    }
}
