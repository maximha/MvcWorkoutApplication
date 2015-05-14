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

        public static ItemTaskModel storageTaskPropertyResult(string _workoutName , string _taskName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            task _task = db.tasks.Where(x => x.workoutName == _workoutName && x.taskName == _taskName).SingleOrDefault();

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

        public static storageModelList storagelistRessult()
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            List<workout> storageList = db.workouts.Where(s => s.inStorage == true).ToList();
            for (int i = 0; i < storageList.Count; i++)
            {
                storageList[i].userName = CryptHelper.Encrypt(storageList[i].userName);
                storageList[i].workoutName = CryptHelper.Encrypt(storageList[i].workoutName);
                storageList[i].inStorage = storageList[i].inStorage;
            }
            db.Dispose();

            storageModelList storageModel = new storageModelList
            {
                result = true,
                storageWorkouts = storageList
            };
            return storageModel;
        }

        public static favoritesModelList favoriteslistRessult(string masterUserName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            List<favorite> workoutsList = db.favorites.Where(s => s.masterUser == masterUserName).ToList();
            for (int i = 0; i < workoutsList.Count; i++)
            {
                workoutsList[i].masterUser = CryptHelper.Encrypt(workoutsList[i].masterUser);
                workoutsList[i].userName = CryptHelper.Encrypt(workoutsList[i].userName);
                workoutsList[i].workoutName = CryptHelper.Encrypt(workoutsList[i].workoutName);
            }
            db.Dispose();

            favoritesModelList favoritesModel = new favoritesModelList
            {
                result = true,
                favoritesWorkouts = workoutsList
            };
            return favoritesModel;
        }

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