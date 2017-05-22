﻿using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Tnf.Web.Models;
using Tnf.Architecture.Dto;
using System.Net;
using Tnf.Architecture.Web.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Tnf.Architecture.Dto.ValueObjects;
using System.Collections.Generic;
using Tnf.Dto;
using System.Linq;
using Tnf.Architecture.Dto.WhiteHouse;
using Tnf.Architecture.Domain.WhiteHouse;

namespace Tnf.Architecture.Web.Tests.Tests
{
    public class WhiteHouseControllerTests : AppTestBase
    {
        [Fact]
        public void Should_Resolve_Controller()
        {
            ServiceProvider.GetService<WhiteHouseController>().ShouldNotBeNull();
        }

        [Fact]
        public async Task GetAll_Presidents_With_Success()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<PagingResponseDto<PresidentDto>>>(
                               $"{RouteConsts.WhiteHouse}?pageSize=10",
                               HttpStatusCode.OK
                           );

            // Assert
            Assert.True(response.Success);
            Assert.Equal(response.Result.Data.Count, 6);
            response.Result.Notifications.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAll_Presidents_With_Invalid_Parameters()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}?",
                HttpStatusCode.BadRequest
                );

            response.Success.ShouldBeTrue();
            response.Result.ShouldBe($"Invalid parameter: PageSize");
        }

        [Fact]
        public async Task Get_President_With_Sucess()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<PresidentDto>>(
                               $"{RouteConsts.WhiteHouse}/1",
                               HttpStatusCode.OK
                           );

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response);
            Assert.NotNull(response.Result.Id == "1");
            Assert.NotNull(response.Result.Name == "George Washington");
            Assert.NotNull(response.Result.Address.Number == "12345678");
        }

        [Fact]
        public async Task Get_President_With_Invalid_Parameter_Return_Bad_Request()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}/%20",
                HttpStatusCode.BadRequest
            );

            // Assert
            Assert.True(response.Success);
            Assert.Equal(response.Result, "Invalid parameter: id");
        }

        [Fact]
        public async Task Get_President_When_Not_Exists_Return_Not_Found()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                               $"{RouteConsts.WhiteHouse}/99",
                               HttpStatusCode.NotFound
                           );

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Post_President_With_Success()
        {
            //Arrange
            var presidentDto = new PresidentDto()
            {
                Id = "7",
                Name = "Lula",
                Address = new Address("Rua de teste", "123", "APT 12", new ZipCode("74125306"))
            };

            var presidents = new List<PresidentDto>() { presidentDto };

            // Act
            var response = await PostResponseAsObjectAsync<List<PresidentDto>, AjaxResponse<ResponseDtoBase<List<PresidentDto>>>>(
                $"{RouteConsts.WhiteHouse}",
                presidents,
                HttpStatusCode.OK
            );

            // Assert
            Assert.True(response.Success);
            Assert.True(response.Result.Data.Any(p => p.Name == "Lula"));
        }

        [Fact]
        public async Task Post_President_With_Invalid_Parameter_Return_Bad_Request()
        {
            // Act
            var response = await PostResponseAsObjectAsync<List<PresidentDto>, AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}",
                new List<PresidentDto>(),
                HttpStatusCode.BadRequest
            );

            // Assert
            Assert.True(response.Success);
            response.Result.ShouldBe("Empty parameter: presidents");
        }

        [Fact]
        public async Task Post_Null_President_Return_Bad_Request()
        {
            // Act
            var response = await PostResponseAsObjectAsync<List<PresidentDto>, AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}",
                null,
                HttpStatusCode.BadRequest
            );

            // Assert
            Assert.True(response.Success);
            response.Result.ShouldBe("Invalid parameter: presidents");
        }

        [Fact]
        public async Task Post_Empty_Presidents_Return_Bad_Request()
        {
            // Act
            var response = await PostResponseAsObjectAsync<List<PresidentDto>, AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}",
                new List<PresidentDto>(),
                HttpStatusCode.BadRequest
            );

            // Assert
            Assert.True(response.Success);
            response.Result.ShouldBe("Empty parameter: presidents");
        }

        [Fact]
        public async Task Post_President_With_Invalid_Parameter_And_Return_Notifications()
        {
            // Act
            var response = await PostResponseAsObjectAsync<List<PresidentDto>, AjaxResponse<ResponseDtoBase<List<PresidentDto>>>>(
                $"{RouteConsts.WhiteHouse}",
                new List<PresidentDto>() { new PresidentDto() },
                HttpStatusCode.OK
            );

            // Assert
            Assert.True(response.Success);
            Assert.False(response.Result.Success);
            Assert.True(response.Result.Notifications.Any(a => a.Message == President.Error.PresidentNameMustHaveValue.ToString()));
            Assert.True(response.Result.Notifications.Any(a => a.Message == President.Error.PresidentZipCodeMustHaveValue.ToString()));
        }

        [Fact]
        public async Task Put_President_With_Success()
        {
            //Arrange
            var presidentDto = new PresidentDto("6", "Ronald Reagan", new Address("Rua de teste", "123", "APT 12", new ZipCode("74125306")));

            // Act
            var response = await PutResponseAsObjectAsync<PresidentDto, AjaxResponse<ResponseDtoBase<PresidentDto>>>(
                $"{RouteConsts.WhiteHouse}/6",
                presidentDto,
                HttpStatusCode.OK
            );

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Result.Notifications);
            Assert.Equal(response.Result.Data.Id, "6");
            Assert.Equal(response.Result.Data.Name, "Ronald Reagan");
        }

        [Fact]
        public async Task Put_President_With_Invalid_Parameter_Return_Bad_Request()
        {
            // Act
            var response = await PutResponseAsObjectAsync<PresidentDto, AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}/%20",
                new PresidentDto(),
                HttpStatusCode.BadRequest
            );

            // Assert
            Assert.True(response.Success);
            response.Result.ShouldBe("Invalid parameter: id");
        }

        [Fact]
        public async Task Put_Null_President_Return_Bad_Request()
        {
            // Act
            var response = await PutResponseAsObjectAsync<PresidentDto, AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}/1",
                null,
                HttpStatusCode.BadRequest
            );

            // Assert
            Assert.True(response.Success);
            response.Result.ShouldBe("Invalid parameter: president");
        }

        [Fact]
        public async Task Put_Empty_President_And_Return_Notifications()
        {
            // Act
            var response = await PutResponseAsObjectAsync<PresidentDto, AjaxResponse<ResponseDtoBase<PresidentDto>>>(
                $"{RouteConsts.WhiteHouse}/6",
                new PresidentDto(),
                HttpStatusCode.OK
            );

            // Assert
            Assert.True(response.Success);
            Assert.False(response.Result.Success);
            Assert.True(response.Result.Notifications.Any(a => a.Message == President.Error.PresidentNameMustHaveValue.ToString()));
            Assert.True(response.Result.Notifications.Any(a => a.Message == President.Error.PresidentZipCodeMustHaveValue.ToString()));
        }

        [Fact]
        public async Task Put_President_When_Not_Exists_Return_Notifications()
        {
            //Arrange
            var presidentDto = new PresidentDto("99", "Ronald Reagan", new Address("Rua de teste", "123", "APT 12", new ZipCode("74125306")));

            // Act
            var response = await PutResponseAsObjectAsync<PresidentDto, AjaxResponse<ResponseDtoBase<PresidentDto>>>(
                $"{RouteConsts.WhiteHouse}/99",
                presidentDto,
                HttpStatusCode.NotFound
            );

            // Assert
            Assert.True(response.Success);
            Assert.False(response.Result.Success);
            Assert.True(response.Result.Notifications.Any(a => a.Message == President.Error.CouldNotFindPresident.ToString()));
        }

        [Fact]
        public async Task Delete_President_With_Success()
        {
            // Act
            var response = await DeleteResponseAsObjectAsync<AjaxResponse<ResponseDtoBase>>(
                $"{RouteConsts.WhiteHouse}/1",
                HttpStatusCode.OK
            );

            // Assert
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Delete_President_With_Invalid_Parameter_Return_Bad_Request()
        {
            // Act
            var response = await DeleteResponseAsObjectAsync<AjaxResponse<string>>(
                $"{RouteConsts.WhiteHouse}/%20",
                HttpStatusCode.BadRequest
            );

            // Assert
            response.Success.ShouldBeTrue();
            response.Result.ShouldBe("Invalid parameter: id");
        }

        [Fact]
        public async Task Delete_President_When_Not_Exists_Return_Notifications()
        {
            // Act
            var response = await DeleteResponseAsObjectAsync<AjaxResponse<ResponseDtoBase>>(
                $"{RouteConsts.WhiteHouse}/99",
                HttpStatusCode.NotFound
            );

            // Assert
            Assert.True(response.Success);
            Assert.False(response.Result.Success);
            Assert.True(response.Result.Notifications.Any(a => a.Message == President.Error.CouldNotFindPresident.ToString()));
        }
    }
}
