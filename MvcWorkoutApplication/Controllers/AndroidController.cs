using MvcWorkoutApplication.Api_Models;
using MvcWorkoutApplication.Helpers;
using MvcWorkoutApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MvcWorkoutApplication.Controllers
{
    public class AndroidController : ApiController
    {
        #region API
        //login function
        [System.Web.Http.HttpPost]
        public AbstractModel Login(LoginModel loginModel)
        {
            user user = checkLogin(loginModel.userName, loginModel.password);

            if (user == null)
            {
                return ResultHelper.boolResult(false, "User not found");
            }
            return ResultHelper.loginResult(loginModel.publicKey);
        }

        [System.Web.Http.HttpPost]
        public AbstractModel RegistrationKey(LoginModel loginModel)
        {

            return ResultHelper.loginResult(loginModel.publicKey);
        }

        [System.Web.Http.HttpPost]
        public AbstractModel Registration(RegistrationModel regModel)
        {
            RegistrationModel decryptedModel = new RegistrationModel();
            decryptedModel.firstName = CryptHelper.Decrypt(regModel.firstName);
            decryptedModel.lastName = CryptHelper.Decrypt(regModel.lastName);
            decryptedModel.userName = CryptHelper.Decrypt(regModel.userName);
            decryptedModel.password = CryptHelper.Decrypt(regModel.password);

            //try to add new user
            bool didAddUser = addUser(decryptedModel.firstName, decryptedModel.lastName, decryptedModel.userName, decryptedModel.password);
            //check if new user added
            if (!didAddUser)
            {
                return ResultHelper.boolResult(false, "Registration failure");
            }

            return ResultHelper.boolResult(true, "Registration success");
        }

        [System.Web.Http.HttpPost]
        public AbstractModel ChangeProfile(RegistrationModel changeProfModel)
        {
            RegistrationModel decryptedModel = new RegistrationModel();
            decryptedModel.firstName = CryptHelper.Decrypt(changeProfModel.firstName);
            decryptedModel.lastName = CryptHelper.Decrypt(changeProfModel.lastName);
            decryptedModel.userName = CryptHelper.Decrypt(changeProfModel.userName);
            decryptedModel.password = CryptHelper.Decrypt(changeProfModel.password);

            //check if user exist in DB
            user _user = findUser(decryptedModel.userName);
            if (_user == null)
            {
                return ResultHelper.boolResult(false, "User not found");
            }
            //try to change profile
            bool didChangeProf = changeProfile(decryptedModel.firstName, decryptedModel.lastName, decryptedModel.userName, decryptedModel.password);
            if (!didChangeProf)
            {
                return ResultHelper.boolResult(false, "Change Profile failure");
            }

            return ResultHelper.boolResult(true, "Change Profile success");
        }

        //AddWorkout function
        [System.Web.Http.HttpPost]
        public AbstractModel AddWorkout(WorkoutModel workoutModel)
        {
            WorkoutModel decryptedModel = new WorkoutModel();
            decryptedModel.userName = CryptHelper.Decrypt(workoutModel.userName);
            decryptedModel.workoutName = CryptHelper.Decrypt(workoutModel.workoutName);

            //check if workoutName allow to workout
            workout _workoutName = checkWorkout(decryptedModel.workoutName);
            if (_workoutName == null)
            {
                //try to add new workout
                bool didAddWorkout = addWorkout(decryptedModel.userName, decryptedModel.workoutName);
                if (didAddWorkout)
                {
                    return ResultHelper.boolResult(true, "Adding Workout success");
                }
                return ResultHelper.boolResult(false, "Adding Workout failure");
            }
            return ResultHelper.boolResult(false, "Workout already exist");
        }

        //AddTask function
        [System.Web.Http.HttpPost]
        public AbstractModel AddTask(TaskModel taskModel)
        {
            TaskModel decryptedModel = new TaskModel();
            decryptedModel.workoutName = CryptHelper.Decrypt(taskModel.workoutName);
            decryptedModel.taskName = CryptHelper.Decrypt(taskModel.taskName);
            decryptedModel.descriptionTask = CryptHelper.Decrypt(taskModel.descriptionTask);
            decryptedModel.timeTask = CryptHelper.Decrypt(taskModel.timeTask);
            decryptedModel.revTask = CryptHelper.Decrypt(taskModel.revTask);

            //check if taskName allow to task
            task _task = checkTask(decryptedModel.taskName);
            if (_task == null)
            {
                //try to add new task
                bool didAddTask = addTask(decryptedModel.workoutName, decryptedModel.taskName, decryptedModel.descriptionTask, decryptedModel.timeTask, decryptedModel.revTask);
                if (didAddTask)
                {
                    return ResultHelper.boolResult(true, "Adding Task success");
                }
                return ResultHelper.boolResult(false, "Adding Task failure");
            }
            return ResultHelper.boolResult(false, "TaskName already exist");
        }
        // Add to Storage
        [System.Web.Http.HttpPost]
        public AbstractModel AddToStorage(StorageModel storageModel) 
        {
            StorageModel decryptedModel = new StorageModel();
            decryptedModel.userName = CryptHelper.Decrypt(storageModel.userName);
            decryptedModel.workoutName = CryptHelper.Decrypt(storageModel.workoutName);
            decryptedModel.inStorage = storageModel.inStorage;

            //check if workoutName allow to workout
            workout _workoutName = checkWorkout(decryptedModel.workoutName);
            if (_workoutName != null)
            {
                //try to add new workout
                bool didAddToStorage = addToStorage(decryptedModel.userName, decryptedModel.workoutName, decryptedModel.inStorage);
                if (didAddToStorage)
                {
                    return ResultHelper.boolResult(true, "Adding Workout To Storage success");
                }
                return ResultHelper.boolResult(false, "Adding Workout To Storage failure");
            }
            return ResultHelper.boolResult(false, "Workout already in the Storage");
            
        }

        //AddWorkout to Favorites function
        [System.Web.Http.HttpPost]
        public AbstractModel AddWorkoutToFavorites(FavoritesModel workoutModel)
        {
            FavoritesModel decryptedModel = new FavoritesModel();
            decryptedModel.masterUserName = CryptHelper.Decrypt(workoutModel.masterUserName);
            decryptedModel.userName = CryptHelper.Decrypt(workoutModel.userName);
            decryptedModel.workoutName = CryptHelper.Decrypt(workoutModel.workoutName);

            //check if workoutName allow to workout
            favorite _favoriteWorkout = checkFavoriteWorkout(decryptedModel.masterUserName, decryptedModel.userName, decryptedModel.workoutName);
            if (_favoriteWorkout == null)
            {
                //try to add new workout
                bool didAddWorkout = addWorkoutToTableFavorites(decryptedModel.masterUserName, decryptedModel.userName, decryptedModel.workoutName);
                if (didAddWorkout)
                {
                    return ResultHelper.boolResult(true, "Adding Workout success");
                }
                return ResultHelper.boolResult(false, "Adding Workout failure");
            }
            return ResultHelper.boolResult(false, "Workout already exist in favorites");
        }

        //Delete function
        [System.Web.Http.HttpPost]
        public AbstractModel DeleteWorkout(WorkoutModel workoutModel)
        {
            WorkoutModel decryptedModel = new WorkoutModel();
            decryptedModel.userName = CryptHelper.Decrypt(workoutModel.userName);
            decryptedModel.workoutName = CryptHelper.Decrypt(workoutModel.workoutName);

            //try to delete workout
            bool didDeleteWorkout = deleteWorkout(decryptedModel.userName, decryptedModel.workoutName);
            if (didDeleteWorkout)
            {
                return ResultHelper.boolResult(true, "Deleting Workout success");
            }
            return ResultHelper.boolResult(false, "Deleting Workout failure");
        }

        [System.Web.Http.HttpPost]
        public AbstractModel DeleteWorkoutFromFavoritesList(FavoritesModel favoritesModel)
        {
            FavoritesModel decryptedModel = new FavoritesModel();
            decryptedModel.masterUserName = CryptHelper.Decrypt(favoritesModel.masterUserName);
            decryptedModel.userName = CryptHelper.Decrypt(favoritesModel.userName);
            decryptedModel.workoutName = CryptHelper.Decrypt(favoritesModel.workoutName);

            //try to delete workout
            bool didDeleteWorkout = deleteWorkoutFromList(decryptedModel.masterUserName, decryptedModel.userName, decryptedModel.workoutName);
            if (didDeleteWorkout)
            {
                return ResultHelper.boolResult(true, "Deleting Workout success");
            }
            return ResultHelper.boolResult(false, "Deleting Workout failure");
        }

        [System.Web.Http.HttpPost]
        public AbstractModel DeleteTask(TaskModel taskModel)
        {
            TaskModel decryptedModel = new TaskModel();
            decryptedModel.workoutName = CryptHelper.Decrypt(taskModel.workoutName);
            decryptedModel.taskName = CryptHelper.Decrypt(taskModel.taskName);

            //try to delete task
            bool didDeleteTask = deleteTask(decryptedModel.workoutName, decryptedModel.taskName);
            if (didDeleteTask)
            {
                return ResultHelper.boolResult(true, "Deleting Task success");
            }
            return ResultHelper.boolResult(false, "Deleting Task failure");
        }

        [System.Web.Http.HttpPost]
        public AbstractModel ListOfWorkoutsName(LoginModel loginModel)
        {
            LoginModel decryptedModel = new LoginModel();
            decryptedModel.userName = CryptHelper.Decrypt(loginModel.userName);
            user user = findUser(decryptedModel.userName);
            //user user = findUser(loginModel.userName);
            if (user == null)
            {
                return ResultHelper.boolResult(false, "User not exist !!!");
            }
            //return ResultHelper.workoutsRessult(loginModel.userName);
            return ResultHelper.workoutsRessult(decryptedModel.userName);
            //return ResultHelper.boolResult(true,"gfgf");
        }

        [System.Web.Http.HttpPost]
        public AbstractModel ListOfTaskName(WorkoutModel workoutModel)
        {
            WorkoutModel decryptedModel = new WorkoutModel();
            decryptedModel.userName = CryptHelper.Decrypt(workoutModel.userName);
            decryptedModel.workoutName = CryptHelper.Decrypt(workoutModel.workoutName);

            //get workout by name
            workout _workout = findWorkout(decryptedModel.workoutName);
            //workout _workout = findWorkout(workoutModel.workoutName);
            if (_workout == null)
            {
                return ResultHelper.boolResult(false, "Workout not exist !!!");
            }
            //return ResultHelper.tasksRrssult(workoutModel.workoutName);
            return ResultHelper.tasksRrssult(decryptedModel.workoutName);
        }

        [System.Web.Http.HttpPost]
        public AbstractModel GetStorageWorkoutsList()
        {
            return ResultHelper.storagelistRessult();
        }

        [System.Web.Http.HttpPost]
        public AbstractModel TaskByName(TaskModel taskModel)
        {
            TaskModel decryptedModel = new TaskModel();
            decryptedModel.taskName = CryptHelper.Decrypt(taskModel.taskName);
            

            // get task by name
            task _task = findTask(decryptedModel.taskName);
            //task _task = findTask(taskModel.taskName);
            if (_task == null)
            {
                return ResultHelper.boolResult(false, "Task not exist !!!");
            }
            //return ResultHelper.taskByNameResult(taskModel.taskName);
            return ResultHelper.taskByNameResult(decryptedModel.taskName);
        }


        [System.Web.Http.HttpPost]
        public AbstractModel FavoritesList(LoginModel loginModel)
        {
            LoginModel decryptedModel = new LoginModel();
            decryptedModel.userName = CryptHelper.Decrypt(loginModel.userName);
            user user = findUser(decryptedModel.userName);
            if (user == null)
            {
                return ResultHelper.boolResult(false, "User not exist !!!");
            }
            return ResultHelper.favoriteslistRessult(decryptedModel.userName);
        }

        [System.Web.Http.HttpPost]
        public AbstractModel StorageTaskProperty(TaskModel taskModel)
        {
            TaskModel decryptedModel = new TaskModel();
            decryptedModel.workoutName = CryptHelper.Decrypt(taskModel.workoutName);
            decryptedModel.taskName = CryptHelper.Decrypt(taskModel.taskName);


            // get task by name
            task _task = findTaskByWorkoutName(decryptedModel.workoutName , decryptedModel.taskName);
            //task _task = findTask(taskModel.taskName);
            if (_task == null)
            {
                return ResultHelper.boolResult(false, "Task not exist !!!");
            }
            //return ResultHelper.taskByNameResult(taskModel.taskName);
            return ResultHelper.storageTaskPropertyResult(decryptedModel.workoutName ,decryptedModel.taskName);
        }
        #endregion

        #region
        private static user checkLogin(string userName, string password)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            //get user by userName
            user _user = db.users.Where(x => x.userName == userName).SingleOrDefault();
            db.Dispose();
            //check if user exist and if password is correct
            //if (_user == null || !Equals(user.password, CryptHelper.getHash(password)))
            if (_user == null || !user.Equals(password, password))
            {
                return null;
            }
            return _user;
        }

        private static workout checkWorkout(string workoutName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            //get workout by workoutName
            workout _workout = db.workouts.Where(x => x.workoutName == workoutName).SingleOrDefault();
            db.Dispose();
            //check if workout exist and if workoutName is correct
            if (_workout == null || !workout.Equals(workoutName, workoutName))
            {
                return null;
            }
            return _workout;
        }

        private static favorite checkFavoriteWorkout(string _masterUserName, string _userName, string _workoutName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            //get workout by workoutName
            favorite _workout = db.favorites.Where(x =>x.masterUser == _masterUserName && x.userName == _userName && x.workoutName == _workoutName).SingleOrDefault();
            db.Dispose();
            //check if workout exist and if workoutName is correct
            if (_workout == null )
            {
                return null;
            }
            return _workout;
        }

        private static task checkTask(string taskName)
        {
            //connect to db
            db_appEntities db = new db_appEntities();
            //get workout by workoutName
            task _task = db.tasks.Where(x => x.taskName == taskName).SingleOrDefault();
            db.Dispose();
            //check if workout exist and if workoutName is correct
            if (_task == null || !workout.Equals(taskName, taskName))
            {
                return null;
            }
            return _task;
        }

        //query function to check if user exist in DB
        private static user findUser(string userName)
        {
            db_appEntities db = new db_appEntities();
            //get user by userName
            user user = db.users.Where(x => x.userName == userName).SingleOrDefault();
            //check if user exist
            if (user == null)
            {
                return null;
            }
            db.Dispose();
            return user;
        }

        //query function to check if workout exist in DB
        private static workout findWorkout(string workoutName)
        {
            db_appEntities db = new db_appEntities();
            //get workout by workoutName
            workout _workout = db.workouts.Where(x => x.workoutName == workoutName).SingleOrDefault();
            db.Dispose();
            //check if workout exist
            if (_workout == null)
            {
                return null;
            }
            return _workout;
        }

        //query function to check if task exist in DB
        private static task findTask(string taskName)
        {
            db_appEntities db = new db_appEntities();
            //get task by taskName
            task _task = db.tasks.Where(x => x.taskName == taskName).SingleOrDefault();
            db.Dispose();
            //check if task exist
            if (_task == null)
            {
                return null;
            }
            return _task;
        }

        private static task findTaskByWorkoutName(string workoutName ,string taskName)
        {
            db_appEntities db = new db_appEntities();
            //get task by taskName
            task _task = db.tasks.Where(x => x.workoutName == workoutName && x.taskName == taskName).SingleOrDefault();
            db.Dispose();
            //check if task exist
            if (_task == null)
            {
                return null;
            }
            return _task;
        }

        //query function to add new user to DB
        private static bool addUser(string firstName, string lastName, string userName, string password)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();

            //check if user exist in DB
            user user = findUser(userName);
            if (user != null)
            {
                return false;
            }
            try 
            {
                user = new user();
                //if user not exist create new user
                user.firstName = firstName;
                user.lastName = lastName;
                user.password = password; // CryptHelper.getHash(password);
                user.userName = userName;
                db.users.Add(user);

                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {}
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }

        private static bool addWorkout(string userName, string workoutName)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();

            //check if workout exist in DB
            workout newWorkout = findWorkout(workoutName);
            if (newWorkout != null)
            {
                return false;
            } 
            try 
            {
                newWorkout = new workout();
                //if workout not exist create new workout
                newWorkout.userName = userName;
                newWorkout.workoutName = workoutName;
                newWorkout.inStorage = false;

                db.workouts.Add(newWorkout);
                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {}
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }

        private static bool addWorkoutToTableFavorites(string masterUserName, string userName, string workoutName)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();

            ////check if workout exist in DB
            //favorite newWorkout = findWorkout(workoutName);
            //if (newWorkout != null)
            //{
            //    return false;
            //} 
            try
            {
                favorite newWorkout = new favorite();
                //if workout not exist create new workout
                newWorkout.masterUser = masterUserName;
                newWorkout.userName = userName;
                newWorkout.workoutName = workoutName;

                db.favorites.Add(newWorkout);
                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            { }
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }

        private static bool addTask(string workoutName, string taskName, string descriptionTask, string timeTask, string revTask)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();

            //check if task exist in DB
            task newTask = findTask(taskName);
            if (newTask != null)
            {
                return false;
            }
            try
            {
                newTask = new task();
                //if task not exist create new task
                newTask.workoutName = workoutName;
                newTask.taskName = taskName;
                newTask.description = descriptionTask;
                newTask.time = timeTask;
                newTask.rev = revTask;

                db.tasks.Add(newTask);
                //db.Entry(newTask).State = EntityState.Added;
                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {}
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }

        private static bool addToStorage(string _userName, string _workoutName, bool _inStorage)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();
            workout workoutToUpade = findWorkoutByUsername(_userName, _workoutName);
            if (workoutToUpade == null)
            {
                return false;
            }
            try
            {
                workoutToUpade.inStorage = true;

                db.Entry(workoutToUpade).State = EntityState.Modified;

                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            { }
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }

        //query function to add new user to DB
        private static bool changeProfile(string firstName, string lastName, string userName, string password)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();

            //check if user exist in DB
            user user = findUser(userName);
            if (user == null)
            {
                return false;
            }
            try 
            { 
                //if user exist change user profile
                user.firstName = firstName;
                user.lastName = lastName;
                user.password = password; // CryptHelper.getHash(password);

                db.Entry(user).State = EntityState.Modified;

                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {}
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }
        #endregion

        #region delete
        private static bool deleteWorkout(string userName, string workoutName)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();
            //check if workout exist in DB
            workout delWorkout = findWorkoutByUsername(userName, workoutName);

            List<task> delTaskList = findAllTasks(workoutName);

            if (delWorkout == null && delTaskList == null)
            {
                return false;
            }
            try
            {
                db.Entry(delWorkout).State = EntityState.Deleted;
                db.SaveChanges();
                foreach (task element in delTaskList)
                    db.Entry(element).State = EntityState.Deleted;
                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {}
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }

        private static bool deleteWorkoutFromList(string _mastersUerName, string _userName, string _workoutName)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();
            //check if workout exist in DB
            favorite delWorkout = checkFavoriteWorkout(_mastersUerName, _userName, _workoutName);

            if (delWorkout == null)
            {
                return false;
            }
            try
            {
                db.Entry(delWorkout).State = EntityState.Deleted;
                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            { }
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }
        }

        private static List<task> findAllTasks(string workoutName)
        {
            db_appEntities db = new db_appEntities();
            //get list of tasks by workoutName
            List<task> listAllTask = db.tasks.Where(x => x.workoutName == workoutName).ToList();//Select(s => s.taskName).ToList();
            db.Dispose();
            //check if list not null
            if (listAllTask == null)
            {
                return null;
            }
            return listAllTask;
        }
        private static bool deleteTask(string workoutName, string taskName)
        {
            bool flag = false;
            //connect to db
            db_appEntities db = new db_appEntities();

            //check if task exist in DB
            task delTask = findTask(taskName);
            if (delTask == null)
            {
                return false;
            }
            
            try
            {
                    db.Entry(delTask).State = EntityState.Deleted;
                    db.SaveChanges();
                    flag = true;
            }
            catch (Exception e)
            {}
            if (flag == true)
                return true;
            else
            {
                db.Dispose();
                return false;
            }

        }
        //query function to check if workout exist in DB
        private static workout findWorkoutByUsername(string _userName, string _workoutName)
        {
            db_appEntities db = new db_appEntities();
            //get workout by workoutName and userName
            workout _workout = db.workouts.Where(x => x.workoutName == _workoutName && x.userName == _userName).SingleOrDefault();
            db.Dispose();
            //check if workout exist
            if (_workout == null)
            {
                return null;
            }
            return _workout;
        }
        #endregion

        
    }
}