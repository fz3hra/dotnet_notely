using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;
using Microsoft.AspNetCore.SignalR;

namespace dotnet_notely.Configurations;

public class NoteHub: Hub
{
    public async Task SendMessage(int noteId, UpdateNoteDto note)
    {
        await Clients.All.SendAsync("ReceiveMessage", noteId, note);
    }
    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        await base.OnConnectedAsync();
    }
}