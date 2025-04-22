using AutoMapper;
using ItemService.Data;
using ItemService.Dtos;
using ItemService.Models;
using System.Text.Json;

namespace ItemService.EventProcessor;

public class ProcessaEvento : IProcessaEvento
{
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;
    public ProcessaEvento(IMapper mapper, IServiceScopeFactory scopeFactory)
    {
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    public void Processa(string mensagem)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            //Esse evento vai rodar em Background por isso tenho que pegar a
            //instancia do repositorio via escopo, nao da pra injetar a dependencia
            //dentro do construtor da classe
            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();

            var restauranteReadDto = JsonSerializer.Deserialize<RestauranteReadDto>(mensagem);

            var restaurante = _mapper.Map<Restaurante>(restauranteReadDto);

            if (!itemRepository.ExisteRestauranteExterno(restaurante.Id))
            {
                itemRepository.CreateRestaurante(restaurante);
                itemRepository.SaveChanges();
                Console.WriteLine("Restaurante salvo no banco");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao processar evento");
            Console.WriteLine(ex.Message);
        }        
    }
}
