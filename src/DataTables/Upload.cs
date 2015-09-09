﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace DataTables
{
    public class Upload
    {
        /*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *
         * Public parameters
         */

        /// <summary>
        /// Database upload options for the 'fields' option in the 'Db()' method.
        /// These are used to provide easy information about the file that will be
        /// stored in the database.
        /// </summary>
        public enum DbType
        {
            /// <summary>
            /// Binary information
            /// </summary>
            Content,

            /// <summary>
            /// Content type
            /// </summary>
            ContentType,

            /// <summary>
            /// File extension (note that this includes the dot)
            /// </summary>
            Extn,

            /// <summary>
            /// File name (with extension)
            /// </summary>
            FileName,

            /// <summary>
            /// File size (bytes)
            /// </summary>
            FileSize,

            /// <summary>
            /// MIME type (same as content type)
            /// </summary>
            MimeType,

            /// <summary>
            /// HTTP path to the file this is computed from
            /// Request.PhysicalApplicationPath . If you are storing the files outside
            /// of your application, this option isn't particularly useful!
            /// </summary>
            WebPath,

            /// <summary>
            /// System path to the file (i.e. the absolute path on your hard disk)
            /// </summary>
            SystemPath
        }


        /*  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *  *
         * Private parameters
         */
        private string _actionStr;
        private Func<HttpPostedFile, dynamic, dynamic> _actionFn;
        private IEnumerable<string> _extns;
        private string _extnError;
        private string _dbTable;
        private string _dbPKey;
        private Dictionary<string, object> _dbFields;
        private readonly List<Func<HttpPostedFile, string>> _validators = new List<Func<HttpPostedFile, string>>();
        private string _error;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Constructor
         */

        /// <summary>
        /// Upload constructor
        /// </summary>
        public Upload()
        { }

        /// <summary>
        /// Upload constructor with a path action
        /// </summary>
        /// <param name="action">Location for where to store the file. This should be
        /// an absolute path on your system.</param>
        public Upload(string action)
        {
            Action(action);
        }

        /// <summary>
        /// Upload constructor with a function action
        /// </summary>
        /// <param name="action">Callback function that is executed when a file
        /// is uploaded.</param>
        public Upload(Func<HttpPostedFile, dynamic, dynamic> action)
        {
            Action(action);
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Public methods
         */

        /// <summary>
        /// Set the action to take when a file is uploaded. As a string the value
        /// given is the full system path to where the uploaded file is written to.
        /// The value given can include three "macros" which are replaced by the
        /// script dependent on the uploaded file:
        /// 
        /// * '__EXTN__' - the file extension (with the dot)
        /// * '__NAME__' - the uploaded file's name (including the extension)
        /// * '__ID__' - Database primary key value if the 'Db()' method is used
        /// </summary>
        /// <param name="action">Full system path for where the file should be stored</param>
        /// <returns>Self for chaining</returns>
        public Upload Action(string action)
        {
            _actionStr = action;
            _actionFn = null;

            return this;
        }

        /// <summary>
        /// Set the action to take when a file is uploaded. As a function the callback
        /// is given the responsiblity of what to do with the uploaded file. That will
        /// typically involve writing it to the file system so it can be used later.
        /// </summary>
        /// <param name="action">Callback</param>
        /// <returns>Self for chaining</returns>
        public Upload Action(Func<HttpPostedFile, dynamic, dynamic> action)
        {
            _actionStr = null;
            _actionFn = action;

            return this;
        }

        /// <summary>
        /// A list of valid file extensions that can be uploaded. This is for simple
        /// validation that the file is of the expected type. The check is
        /// case-insensitive. If no extensions are given, no validation is performed
        /// on the file extension.
        /// </summary>
        /// <param name="extns">List of extensions to test against.</param>
        /// <param name="error">Error message for if the file is not valid.</param>
        /// <returns>Self for chaining</returns>
        public Upload AllowedExtensions(IEnumerable<string> extns, string error = "This file type cannot be uploaded")
        {
            _extns = extns;
            _extnError = error;

            return this;
        }

        /// <summary>
        /// Database configuration method. When used, this method will tell Editor
        /// what information you want to be wirtten to a database on file upload, should
        /// you wish to store relational information about your files on the database
        /// (this is generally recommended).
        /// </summary>
        /// <param name="table">Name of the table where the file information should be stored</param>
        /// <param name="pkey">Primary key column name. This is required so each row can
        /// be uniquely identified.</param>
        /// <param name="fields">A list of the fields to be wirtten to on upload. The
        /// dictonary keys are used as the database column names and the values can be
        /// defined by the 'DbType' enum of this class. The value can also be a string,
        /// which will be written directly to the database, or a function which will be
        /// executed and the returned value written to the database.</param>
        /// <returns>Self for chanining</returns>
        public Upload Db(string table, string pkey, Dictionary<string, object> fields)
        {
            _dbTable = table;
            _dbPKey = pkey;
            _dbFields = fields;

            return this;
        }

        /// <summary>
        /// Add a validation method to check file uploads. Multiple validators can be
        /// added by calling this method multiple times. They will be executed in
        /// sequence when a file has been uploaded.
        /// </summary>
        /// <param name="fn">Validation function. The function takes a single parameter,
        /// an HttpPostedFile, and a string is returned on error with the error message.
        /// If the validation does not fail, 'null' should be returned.</param>
        /// <returns></returns>
        public Upload Validator(Func<HttpPostedFile, string> fn)
        {
            _validators.Add(fn);

            return this;
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Internal methods
         */

        /// <summary>
        /// Get the error message for the uplaod
        /// </summary>
        /// <returns>Error message</returns>
        internal string Error()
        {
            return _error;
        }

        /// <summary>
        /// Execute an upload
        /// </summary>
        /// <param name="editor">Host editor</param>
        /// <returns>Id of the new file</returns>
        internal dynamic Exec(Editor editor)
        {
            dynamic id = null;
            var files = editor.Request().Files;
            var upload = files["upload"];

            // Validation of input files
            if (upload == null)
            {
                _error = "No file uploaded";
                return false;
            }

            // NOTE handling errors where the file size uploaded is larger than
            // that allowed must be handled in Global.aspx.cs
            // http://stackoverflow.com/questions/2759193

            // Validation - acceptable files extensions
            if (_extns != null && _extns.Any())
            {
                var extension = Path.GetExtension(upload.FileName).ToLower();

                if (_extns.Select(x => x.ToLower()).ToList().Contains(extension) == false)
                {
                    _error = _extnError;
                    return false;
                }
            }

            // Validation - custom callbacks
            foreach (var validator in _validators)
            {
                var res = validator(upload);

                if (res != null)
                {
                    _error = res;
                    return false;
                }
            }

            // Commit to the database
            if (_dbTable != null)
            {
                id = _dbExec(editor, upload);
            }

            // Perform file system actions
            return _actionExec(id, upload);
        }

        /// <summary>
        /// Get the primary key of the files table
        /// </summary>
        /// <returns>Primary key column</returns>
        internal string Pkey()
        {
            return _dbPKey;
        }

        /// <summary>
        /// Get the table name for the files table
        /// </summary>
        /// <returns>Table name</returns>
        internal string Table()
        {
            return _dbTable;
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Private methods
         */

        /// <summary>
        /// Execute the configured action for the upload
        /// </summary>
        /// <param name="id">Primary key value</param>
        /// <param name="upload">Posted file</param>
        /// <returns>File identifier - typically the primary key</returns>
        private dynamic _actionExec(dynamic id, HttpPostedFile upload)
        {
            if (_actionStr == null)
            {
                // Custom function
                return _actionFn(upload, id);
            }

            // Default action - move the file to the location specified by the
            // action string
            var to = _path(upload.FileName, id);

            try
            {
                upload.SaveAs(to);
            }
            catch (Exception e)
            {
                _error = "Error saving file. " + e.Message;
                return false;
            }

            return id ?? to;
        }

        /// <summary>
        /// Add a record to the database for a newly uploaded file
        /// </summary>
        /// <param name="editor">Host editor</param>
        /// <param name="upload">Uploaded file</param>
        /// <returns>Primary key value for the newly uploaded file</returns>
        private dynamic _dbExec(Editor editor, HttpPostedFile upload)
        {
            var db = editor.Db();
            var pathFields = new Dictionary<string, DbType>();

            // Insert the details requested, for the columns requested
            var q = db.Query("insert")
                .Table(_dbTable);

            foreach (var pair in _dbFields)
            {
                var column = pair.Key;
                var prop = pair.Value;

                if (prop is DbType)
                {
                    var propType = (DbType)prop;

                    switch (propType)
                    {
                        case DbType.Content:
                            q.Set(column, upload.ToString());
                            break;

                        case DbType.ContentType:
                        case DbType.MimeType:
                            q.Set(column, upload.ContentType);
                            break;

                        case DbType.Extn:
                            q.Set(column, Path.GetExtension(upload.FileName));
                            break;

                        case DbType.FileName:
                            q.Set(column, upload.FileName);
                            break;

                        case DbType.FileSize:
                            q.Set(column, upload.ContentLength);
                            break;

                        case DbType.SystemPath:
                            pathFields.Add(column, DbType.SystemPath);
                            q.Set(column, "-"); // Use a temporary value to avoid cases
                            break; // where the db will reject empty values

                        case DbType.WebPath:
                            pathFields.Add(column, DbType.WebPath);
                            q.Set(column, "-"); // Use a temporary value (as above)
                            break;

                        default:
                            throw new Exception("Unknown database type");
                    }
                }
                else
                {
                    try
                    {
                        // Callable function - execute to get the value
                        var propFn = (Func<Database, HttpPostedFile, dynamic>)prop;
                        q.Set(column, propFn(db, upload));
                    }
                    catch (Exception)
                    {
                        // Not a function, so use the value as it is given
                        q.Set(column, prop);
                    }
                }
            }

            var res = q.Exec();
            var id = res.InsertId();

            // Update the newly inserted row with the path information. We have to
            // use a second statement here as we don't know in advance what the
            // database schema is and don't want to prescribe that certain triggers
            // etc be created. It makes it a bit less efficient but much more
            // compatible
            if (pathFields.Any())
            {
                var path = _path(upload.FileName, id);
                var physicalPath = editor.Request().PhysicalApplicationPath ?? "";
                var webPath = path.Replace(physicalPath, "");

                var pathQ = db
                    .Query("update")
                    .Table(_dbTable)
                    .Where(_dbPKey, id);

                foreach (var pathField in pathFields)
                {
                    pathQ.Set(pathField.Key, pathField.Value == DbType.WebPath ? webPath : path);
                }

                pathQ.Exec();
            }

            return id;
        }

        /// <summary>
        /// Apply macros to a user specified path
        /// </summary>
        /// <param name="name">File path</param>
        /// <param name="id">Primary key value for the file</param>
        /// <returns>Resolved path</returns>
        private string _path(string name, string id)
        {
            return _actionStr
                .Replace("__NAME__", Path.GetFileNameWithoutExtension(name))
                .Replace("__ID__", id)
                .Replace("__EXTN__", Path.GetExtension(name));
        }
    }
}