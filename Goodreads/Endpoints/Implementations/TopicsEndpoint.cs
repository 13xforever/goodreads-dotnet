using Goodreads.Helpers;
using Goodreads.Http;
using Goodreads.Models.Request;
using Goodreads.Models.Response;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goodreads.Clients
{
    internal sealed class TopicsEndpoint : Endpoint, IOAuthTopicsEndpoint
    {
        public TopicsEndpoint(IConnection connection)
            : base(connection)
        {
        }

        public async Task<Topic> GetInfo(long topicId)
        {
            var endpoint = $"topic/show?id={topicId}";
            return await Connection.ExecuteRequest<Topic>(endpoint, null, null, "topic").ConfigureAwait(false);
        }

        public async Task<PaginatedList<Topic>> GetTopics(
            long folderId,
            long groupId,
            int page = 1,
            GroupFolderSort sort = GroupFolderSort.Title,
            OrderInfo order = OrderInfo.Asc)
        {
            var endpoint = $"topic/group_folder/{folderId}";

            var parameters = new[]
            {
                 new Parameter("group_id", groupId, ParameterType.QueryString),
                 new Parameter("page", page, ParameterType.QueryString),
                 new Parameter(EnumHelpers.QueryParameterKey<GroupFolderSort>(), EnumHelpers.QueryParameterValue(sort), ParameterType.QueryString),
                 new Parameter(EnumHelpers.QueryParameterKey<OrderInfo>(), EnumHelpers.QueryParameterValue(order), ParameterType.QueryString),
            };

            return await Connection.ExecuteRequest<PaginatedList<Topic>>(endpoint, parameters, null, "group_folder/topics").ConfigureAwait(false);
        }

        public async Task<PaginatedList<Topic>> GetUnreadTopics(
            long groupId,
            bool viewed = false,
            int page = 1,
            GroupFolderSort sort = GroupFolderSort.Title,
            OrderInfo order = OrderInfo.Asc)
        {
            var endpoint = $"topic/unread_group/{groupId}";

            var parameters = new List<Parameter>
            {
                new Parameter("page", page, ParameterType.QueryString),
                new Parameter(EnumHelpers.QueryParameterKey<GroupFolderSort>(), EnumHelpers.QueryParameterValue(sort), ParameterType.QueryString),
                new Parameter(EnumHelpers.QueryParameterKey<OrderInfo>(), EnumHelpers.QueryParameterValue(order), ParameterType.QueryString),
            };

            if (viewed)
            {
                parameters.Add(new Parameter("viewed", viewed, ParameterType.QueryString));
            }

            return await Connection.ExecuteRequest<PaginatedList<Topic>>(endpoint, parameters, null, "group_folder/topics").ConfigureAwait(false);
        }

        public async Task<Topic> CreateTopic(
            TopicSubjectType type,
            long subjectId,
            long? folderId,
            string title,
            bool isQuestion,
            string comment,
            bool addToUpdateFeed,
            bool needDigest)
        {
            var endpoint = "topic";

            var parameters = new List<Parameter>
            {
                 new Parameter(EnumHelpers.QueryParameterKey<TopicSubjectType>(), EnumHelpers.QueryParameterValue(type), ParameterType.QueryString),
                 new Parameter("topic[subject_id]", subjectId, ParameterType.QueryString),
                 new Parameter("topic[title]", title, ParameterType.QueryString),
                 new Parameter("topic[question_flag]", isQuestion ? "1" : "0", ParameterType.QueryString),
                 new Parameter("comment[body_usertext]", comment, ParameterType.QueryString),
            };

            if (folderId.HasValue)
            {
                parameters.Add(new Parameter("topic[folder_id]", folderId.Value, ParameterType.QueryString));
            }

            if (addToUpdateFeed)
            {
                parameters.Add(new Parameter("update_feed", "on", ParameterType.QueryString));
            }

            if (needDigest)
            {
                parameters.Add(new Parameter("digest", "on", ParameterType.QueryString));
            }

            return await Connection.ExecuteRequest<Topic>(endpoint, parameters, null, "topic", Method.POST);
        }
    }
}
