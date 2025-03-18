using AutoMapper;
using Server.API.Models;
using Server.API.Models.Users;
using Server.Data;
using Server.Models.ChapterDtos;
using Server.Models.CommunityFeedDtos;
using Server.Models.ContentDtos;
using Server.Models.CourseDepartmentDtos;
using Server.Models.CourseDtos;
using Server.Models.DepartmentDtos;
using Server.Models.DetailesDtos;
using Server.Models.DetailsDtos;
using Server.Models.FacultyDtos;
using Server.Models.LecturerDtos;
using Server.Models.User;
using Server.Models.UserDeviceDtos;

namespace Server.API.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {

            CreateMap<ApiUser, LoginDto>().ReverseMap();
            CreateMap<ApiUser, ApiUserDto>().ReverseMap();
            CreateMap<ApiUser, RedeemQRCodeDto>().ReverseMap();
            CreateMap<ApiUser, UserResponseDto>().ReverseMap();
            CreateMap<ApiUser, RedeemQrCodeResDto>().ReverseMap();


            CreateMap<ApiUser, UserResponseDto>()
           .ForMember(dest => dest.QRCode, opt => opt.MapFrom(src => src.QRCode));

            CreateMap<ApiUser, RedeemQRCodeDto>()
                .ForMember(dest => dest.QRCode, opt => opt.MapFrom(src => src.QRCode))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points));







            CreateMap<Course, CreateCourseDto>().ReverseMap();
            CreateMap<Course, UpdateCourseDto>().ReverseMap();
            CreateMap<Course, CourseDto>().ReverseMap();

            CreateMap<Course, GetCourseDto>().ReverseMap();
            CreateMap<Course, CourseResponsesDto>()
            .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturers.FirstOrDefault().LecturerName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Lecturers.FirstOrDefault().Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Lecturers.FirstOrDefault().Image))
            .ForMember(dest => dest.Chapters, opt => opt.MapFrom(src => src.Chapters.Select(chapter => new ChapterDetailDto
            {
                ChapterId = chapter.ChapterId,
                ChapterName = chapter.ChapterName,
                ChapterOrder = chapter.ChapterOrder,
                ContentType = chapter.Contents.FirstOrDefault().ContentType, // Assuming first content's type
                ContentTitle = chapter.Contents.FirstOrDefault().ContentTitle // Assuming first content's title
            }).ToList()));
            CreateMap<Chapter, LearnChapterDto>()
                .ForMember(dest => dest.ChapterId, opt => opt.MapFrom(src => src.ChapterId))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.ChapterName))
                .ForMember(dest => dest.ChapterOrder, opt => opt.MapFrom(src => src.ChapterOrder))
                .ForMember(dest => dest.Contents, opt => opt.MapFrom(src =>
                    src.Contents.Any() ? src.Contents.Select(c => new ContentDto
                    {
                        ContentId = c.ContentId,
                        ChapterId = c.ChapterId, 
                        ContentType = c.ContentType,
                        ContentTitle = c.ContentTitle,
                        ContentUrl = c.ContentUrl, 
                        ContentOrder = c.ContentOrder
                        
                    }).ToList() : new List<ContentDto>()));




            CreateMap<CourseResponsesDto, CourseProgressDto>()
                    .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => src.Progress));
                






            CreateMap<CommunityFeed, CommunityFeedDto>().ReverseMap();
            CreateMap<CommunityFeed, CreateCommunityFeedDto>().ReverseMap();
            CreateMap<CommunityFeed, GetCommunityFeedDto>().ReverseMap();
            CreateMap<CommunityFeed, UpdateCommunityFeedDto>().ReverseMap();

            CreateMap<CommunityFeed, CommunityFeedResponseDto>()
    .ForMember(dest => dest.answers, opt => opt.MapFrom(src => src.Answers));

            
            CreateMap<Answer, AnswerDto>();
            CreateMap<Answer, CreateAnswerDto>();


            CreateMap<Department, CreateDepartmentDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, GetDepartmentDto>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDto>().ReverseMap();
            CreateMap<Faculty, FacultyDetailsDto>()
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.FacultyName))
                .ForMember(dest => dest.Departments, opt => opt.MapFrom(src =>
                    src.Departments.Select(d => new DepartmentDetails
                    {
                        DepartmentId = d.DepartmentId,
                        DepartmentName = d.DepartmentName
                    }).ToList()));

            CreateMap<Content, ContentDto>().ReverseMap();
            CreateMap<Content, GetContentDto>().ReverseMap();
            CreateMap<Content, CreateContentDto>().ReverseMap();
            CreateMap<Content, UpdateContentDto>().ReverseMap();


            CreateMap<Chapter, ChapterDto>().ReverseMap();
            CreateMap<Chapter, GetChapterDto>().ReverseMap();
            CreateMap<Chapter, CreateChapterDto>().ReverseMap();
            CreateMap<Chapter, UpdateChapterDto>().ReverseMap();
            

            CreateMap<CourseDepartment, CourseDepartmentDto>().ReverseMap();
            CreateMap<CourseDepartment, CreateCourseDepartmentDto>().ReverseMap();
            CreateMap<CourseDepartment, GetCourseDepartmentDto>().ReverseMap();
            CreateMap<CourseDepartment, UpdateCourseDepartmentDto>().ReverseMap();

        
            CreateMap<Lecturer, CreateCourselecturerDto>().ReverseMap();
            CreateMap<Lecturer, GetCourselecturerDto>().ReverseMap();
            CreateMap<Lecturer, UpdateCourselecturerDto>().ReverseMap();
            CreateMap<Lecturer, CourselecturerDto>().ReverseMap();
           


            CreateMap<Detail, CreateDetailsDto>().ReverseMap();
            CreateMap<Detail, UpdateDetailsDto>().ReverseMap();
            CreateMap<Detail, DetailsDto>().ReverseMap();
            CreateMap<Detail, GetDetailsDto>().ReverseMap();


            CreateMap<UserDevice, CreateUserDeviceDto>().ReverseMap();
            CreateMap<UserDevice, UserDeviceDto>().ReverseMap();
            CreateMap<UserDevice, GetUserDeviceDto>().ReverseMap();
            CreateMap<UserDevice, UpdateUserDevice>().ReverseMap();


            CreateMap<Faculty, FacultyDtos>().ReverseMap();
            CreateMap<Faculty, UpdateFacultyDtos>().ReverseMap();
            CreateMap<Faculty, CreateFacultyDtos>().ReverseMap();
            CreateMap<Faculty, GetFacultyDtos>().ReverseMap();
            CreateMap<Faculty, FacultyDetailsDto>()
            .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId))
            .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.FacultyName))
            .ForMember(dest => dest.Departments, opt => opt.MapFrom(src =>
                src.Departments.Select(d => new DepartmentDetails
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName
                }).ToList()));


        }
    }
}
