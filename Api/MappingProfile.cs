using AutoMapper;
using Domain.Collection;
using Infrastructure.Collections;
using Infrastructure.Twitter;
using Tweetinvi.Models;

namespace Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IUser, TwitterUserResponseModel>();
            CreateMap<IUser, TwitterSource>();
            CreateMap<CollectionUpdateModel, TwitterSourceCollection>();
        }
    }
}
