using System.Runtime.Serialization;

namespace Messenger
{
    public class ResponseErrors
    {
        public const string USER_ALREADY_EXIST = "User already exist";
        public const string USER_NOT_FOUND = "User not found";
        public const string USER_HAS_NOT_ACCESS = "You have not access to this operation";
        public const string PASSWORD_ALREADY_SET = "This password already set";
        public const string USER_ALREADY_AUTHORIZE = "You already authorize in this device. Use signOut method and after that use this method again";
        public const string USER_EMAIL_NOT_SET = "You must first add an email to your profile, then confirm it";
        public const string EMAIL_ALREADY_EXIST = "This email already exist";
        public const string USER_ALREADY_HAS_CODE = "You already have a code that is not used. if you want to poison the code again, use the method resendCode";
        public const string USER_HAS_NOT_CODE = "You have not confirmation code";
        public const string USER_NOT_PARTICIPANT = "This user is not a member";
        public const string USER_TYPE_NOT_FOUND = "This user type does not exist";

        #region Chats
        public const string CHAT_NOT_FOUND = "Chat not found";
        public const string CHAT_ADMIN_REQUIRED = "You must be an admin in this chat to do this.";
        public const string CHAT_ADMIN_OR_MODER_REQUIRED = "You must be an admin or moderator in this chat to do this.";
        public const string CHAT_ADMIN_NOT_DELETED = "You must be an creator of this chat for kick admin";
        public const string CHAT_MODER_NOT_DELETED = "You must be an administrator of this chat for kick moderator";
        public const string CHAT_PRIVATE = "This chat is private";
        public const string CHAT_ROLE_NOT_FOUND = "Chat role not found";
        public const string USER_ALREADY_IN_CHAT = "One or more users from the invited list are already in the chat";
        public const string USER_LIST_CHATS_IS_EMPTY = "User has zero chat";
        #endregion

        #region Channels
        public const string CHANNEL_NOT_FOUND = "Channel not found";
        public const string CHANNEL_LINK_NOT_FOUND = "Channel link not found";
        public const string USER_ALREADY_IN_CHANNEL = "User already in the chat";
        public const string CHANNEL_LINK_INVALID = "This link is invalid or expired";
        public const string CHANNEL_LINK_ALREADY_USED = "This link already used";
        #endregion

        #region Files
        public const string FILE_IS_EMPTY = "Uploaded file is empty!";
        public const string FILE_NOT_FOUND = "File not found";
        public const string COUNT_FILES_VERY_LONG = "Count files in this request is very long! The maximum number of files in request is five";
        #endregion

        #region Invalid Field
        public const string INVALID_PHONE = "Phonenumber has an incorrect format";
        public const string ALREADY_EXISTS = "Some fields already exists";
        public const string INVALID_PASSWORD = "Password length must be from 10 to 32 chars";
        public const string INVALID_CODE = "Code length must be 6 chars";
        public const string FIELD_LENGTH_IS_LONG = "One or more fields is very long";
        public const string INVALID_FIELDS = "Some fields is invalid";
        #endregion
        public const string TOKEN_EXPIRED = "The token has expired. Update Token";
        public const string UNUSED_CODE_NOT_EXIST = "This unused code does not exist. If you want send new code, use the method sendCode";
        public const string PERMISSION_DENIED = "Your rights do not allow you to perform this operation";

        public const string UNAUTHORIZE = "Unauthorize! To access this resource, sign in to the system(use signIn method)";
        public const string INVALID_ROLE_FOR_OPENATION = "You have invalid role for this operation under this user";

        public const string SESSION_NOT_FOUND = "Session not found";

        public const string SESSION_ALREADY_ENDED = "Session already end. Any session not found";
        public const string USER_NOT_AUTHENTIFICATION = "Current user is not authentification or has been deleted";
        public const string INVALID_TOKEN = "Token is empty or has invalid format";
    }
}
