﻿using RestauranteService.Dtos;
using System.Text;
using System.Text.Json;

namespace RestauranteService.ItemServiceHttpClient;

public class ItemServiceHttpClient : IItemServiceHttpClient
{
    private HttpClient _client { get; }
    private readonly IConfiguration _configuration;
    public ItemServiceHttpClient(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task EnviaRestauranteParaItemServiceAsync(RestauranteReadDto readDto)
    {
        var conteudoHttp = new StringContent
            (
                JsonSerializer.Serialize(readDto),
                Encoding.UTF8,
                "application/json"
            );

        await _client.PostAsync(_configuration["ItemService"], conteudoHttp);
    }
}
