﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoreBot.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CoreBot.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Только капитан может выполнять эту команду.
        /// </summary>
        internal static string CaptainRequiredPermission {
            get {
                return ResourceManager.GetString("CaptainRequiredPermission", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Имя команды было изменено на &apos;{0}&apos;.
        /// </summary>
        internal static string ChangeTeamNameCompletesdMessage {
            get {
                return ResourceManager.GetString("ChangeTeamNameCompletesdMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Команда недоступна. Необходимо создать свою команду..
        /// </summary>
        internal static string ChoicePlayMode {
            get {
                return ResourceManager.GetString("ChoicePlayMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Я хочу играть сам (один или как капитан команды).
        /// </summary>
        internal static string CreateTeam {
            get {
                return ResourceManager.GetString("CreateTeam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ваше имя команды &apos;{0}&apos;. Вы всегда можете изменить имя командой /set_team_name. Если хотите, чтобы к вам присоединились игроки, дайте им пин код &apos;{1}&apos;. Этот код вы можете передать на стенд DotNetRu, чтобы другие участники могли к вам присоединиться.
        /// </summary>
        internal static string CreateTeamCompletedMessage {
            get {
                return ResourceManager.GetString("CreateTeamCompletedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ведите пин код команды (вы можете обратиться за пин кодом доступной команды на стенд DotNetRu).
        /// </summary>
        internal static string InputPinTeamMessage {
            get {
                return ResourceManager.GetString("InputPinTeamMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Введите имя команды.
        /// </summary>
        internal static string InputTeamNameMessage {
            get {
                return ResourceManager.GetString("InputTeamNameMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Я хочу присоединиться к команде. В этом режиме только капитан команды сможет отвечать на вопросы..
        /// </summary>
        internal static string JoinToTeam {
            get {
                return ResourceManager.GetString("JoinToTeam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Пин код не найден. Укажите существующий код.
        /// </summary>
        internal static string PinNotFoundMessage {
            get {
                return ResourceManager.GetString("PinNotFoundMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Немного терпения скоро начнём игру ).
        /// </summary>
        internal static string PleaseWaitStartGame {
            get {
                return ResourceManager.GetString("PleaseWaitStartGame", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Пожалуйста, выбирите один из доступных выборов.
        /// </summary>
        internal static string RetryPromptText {
            get {
                return ResourceManager.GetString("RetryPromptText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Выбери вариант, как ты хочешь играть.
        /// </summary>
        internal static string SelectTeamTypeText {
            get {
                return ResourceManager.GetString("SelectTeamTypeText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Команда уже существует. Выберите другое имя..
        /// </summary>
        internal static string TeamAlreadyExistsMessage {
            get {
                return ResourceManager.GetString("TeamAlreadyExistsMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Вам будет приходить нотификация о вопросах, на которые надо будет ответить капитану команды..
        /// </summary>
        internal static string TeamNotificationInfo {
            get {
                return ResourceManager.GetString("TeamNotificationInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Привет, мой герой! Немного терпения и мы скоро начнём игру..
        /// </summary>
        internal static string WelcomeMessage {
            get {
                return ResourceManager.GetString("WelcomeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Добро пожаловать в комманду &apos;{0}&apos;!.
        /// </summary>
        internal static string WelcomeToTeamMessage {
            get {
                return ResourceManager.GetString("WelcomeToTeamMessage", resourceCulture);
            }
        }
    }
}
