using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Enums
{
    public enum MiraiApiType
    {
        about,
        botList,
        messageFromId,
        friendList,
        groupList,
        memberList,
        botProfile,
        friendProfile,
        memberProfile,
        userProfile,
        sendFriendMessage,
        sendGroupMessage,
        sendTempMessage,
        sendNudge,
        recall,
        roamingMessages,
        file_list,
        file_info,
        file_mkdir,
        file_delete,
        file_move,
        file_rename,
        deleteFriend,
        mute,
        unmute,
        kick,
        quit,
        muteAll,
        unmuteAll,
        setEssence,
        groupConfig_get,
        groupConfig_update,
        memberInfo_get,
        memberInfo_update,
        memberAdmin,
        anno_list,
        anno_publish,
        anno_delete,
        resp_newFriendRequestEvent,
        resp_memberJoinRequestEvent,
        resp_botInvitedJoinGroupRequestEvent
    }
}
