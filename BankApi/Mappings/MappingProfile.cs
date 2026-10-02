using AutoMapper;
using BankApi.Data.Models;
using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.DTOs.UpdateDTOs;
namespace BankApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateClientDto, Client>();
            CreateMap<UpdateClientDto, Client>();
            CreateMap<Client, ClientResponseDto>();

            CreateMap<CreateAccountDto, BankAccount>();
            CreateMap<BankAccount, BankAccountResponseDto>();

            CreateMap<CreatePhoneDto, Phone>();
            CreateMap<Phone, PhoneResponseDto>();

            CreateMap<CreateCardDto, Card>();
            CreateMap<Card,  CardResponseDto>();
            CreateMap<TransactionLog, TransactionLogResponseDto>();
        }
    }
}
