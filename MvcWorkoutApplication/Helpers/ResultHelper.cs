using MvcWorkoutApplication.Api_Models;
using MvcWorkoutApplication.Models;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWorkoutApplication.Helpers
{
    public class ResultHelper
    {
        public static ResultModel boolResult(bool result, string message)
        {
            return new ResultModel()
            {
                result = result,
                message = message
            };
        }
        public static LogModel loginResult( string rsaPublic)
        {
            /*
             * need implement of aes key created and encrypted by rsa key "userKey"
             */
            string aesKey = CryptHelper.generateAESKey();

            Globals.global_AES_Key = aesKey; //????


            LogModel logModel = new LogModel
            {
                result = true,
                publicKey = encodeKey(aesKey, rsaPublic)
            };
            return logModel;
        }

        public static workoutModelList workoutsRessult(string _userName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            List<string> names = db.workouts.Where(s => s.userName == _userName).Select(s => s.workoutName).ToList();
            for (int i = 0; i < names.Count;i++ )
            {
                names[i] = CryptHelper.Encrypt(names[i]);
            }
                db.Dispose();
            workoutModelList workoutModel = new workoutModelList
            {
                result = true,
                workouts = names
            };
            return workoutModel;
        }

        public static taskModelList tasksRrssult(string _workoutName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();

            List<string> names = db.tasks.Where(s => s.workoutName == _workoutName).Select(s => s.taskName).ToList();
            for (int i = 0; i < names.Count; i++)
            {
                names[i] = CryptHelper.Encrypt(names[i]);
            }
            db.Dispose();

            taskModelList taskModel = new taskModelList
            {
                result = true,
                tasksList = names
            };
            return taskModel;
        }

        public static ItemTaskModel taskByNameResult(string _taskName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            task _task = db.tasks.Where(x => x.taskName == _taskName).SingleOrDefault();

            _task.workoutName = CryptHelper.Encrypt(_task.workoutName);
            _task.taskName = CryptHelper.Encrypt(_task.taskName);
            _task.description = CryptHelper.Encrypt(_task.description);
            _task.time = CryptHelper.Encrypt(_task.time);
            _task.rev = CryptHelper.Encrypt(_task.rev);

            db.Dispose();

            ItemTaskModel taskModel = new ItemTaskModel
            {
                result = true,
                itemTask = _task
            };
            return taskModel;
        }

        //=============================================================================================
        public static workoutModelList storagelistRessult()
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            List<String> names = db.workouts.Where(s => s.inStorage == true).Select(s => s.workoutName).ToList();
            /*for (int i = 0; i < names.Count; i++)
            {
                names[i] = CryptHelper.Encrypt(names[i]);
            }*/
            db.Dispose();

            workoutModelList workoutModel = new workoutModelList
            {
                result = true,
                workouts = names
            };
            return workoutModel;
        }

        //=============================================================================================

        private static string encodeKey(string key, string userKey)
        {
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] userKeyBytes = Convert.FromBase64String(userKey);
            RsaPrivateCrtKeyParameters privRSA = null;
            RsaKeyParameters pubRSA = (RsaKeyParameters)PublicKeyFactory.CreateKey(userKeyBytes);
            RSAHelper rsa = new RSAHelper(privRSA, pubRSA);
            byte[] resultBytes = rsa.encrypt(keyBytes);
            return Convert.ToBase64String(resultBytes);
        }
    }
}