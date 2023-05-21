using DevFreela.API.Filters;
using DevFreela.API.Models;
using DevFreela.Application.Commands.CreateProject;
using DevFreela.Application.Services.Implementations;
using DevFreela.Application.Services.Interfaces;
using DevFreela.Application.Validators;
using DevFreela.Core.Repositories;
using DevFreela.Core.Services;
using DevFreela.Infrastructure.Auth;
using DevFreela.Infrastructure.Persistence;
using DevFreela.Infrastructure.Persistence.Repositories;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevFreela.API", Version = "v1" });

    /*Tipo de autenticação de segurança, nome Bearer */
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {// esquema de seguranºa do OpenApi
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,// recebem o token via cabeçalho
        Description = "JWT Authorization header usando o esquema Bearer."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement // tem que utilizar o authorization para realizar o login
                 {
                     {
                           new OpenApiSecurityScheme
                             {
                                 Reference = new OpenApiReference
                                 {
                                     Type = ReferenceType.SecurityScheme,
                                     Id = "Bearer"
                                 }
                             },
                             new string[] {}
                     }
                 });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // defini o esquema da autenticação, como vai ser adicionado o token no cabeçalho
              .AddJwtBearer(options => // adiciona as configurações
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {//valida
                      ValidateIssuer = true, //appseting
                      ValidateAudience = true, //appseting
                      ValidateLifetime = true, //verifica se o token já expirou
                      ValidateIssuerSigningKey = true, // chave de assinarura 

                      ValidIssuer = builder.Configuration["Jwt:Issuer"],
                      ValidAudience = builder.Configuration["Jwt:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                      /*
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey
                      (Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])) // utiliza o algoritimo de chave simetrica de segurança*/
                  };
              });


var connectionString = builder.Configuration.GetConnectionString("DevFreelaCs");

//builder.Services.AddDbContext<DevFreelaDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<DevFreelaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevFreelaCs")));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers(options => options.Filters.Add(typeof(ValidationFilter)))
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUserCommandValidator>());

/*pode ser qualquer tipo que esteja relacionado ao command, nesse caso foi passado o tipo de uma classe que esteja no projeto aplication
 * pega o comando e busca todas as classe desse assemplyu ou seja desse projeto que implementam o que o Mediator especifica para o padrão CQRS, nesse caso o IRequest<tipo de retorno>
 * busca todoas as classes que implementao o IRequest e associa aos comands Handle que implementam IRquestHandler<TipoComand, tipo de respota>
 */
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly));
//builder.Services.AddMediatR(typeof(CreateProjectCommand));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
