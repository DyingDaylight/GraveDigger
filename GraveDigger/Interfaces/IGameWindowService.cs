using GraveDigger.Data;
using GraveDigger.Props;

namespace Interfaces;

public interface IGameWindowService
{
    void OpenTombstoneWindow(Tombstone tombstoneData);
    void CloseCurrentWindow();
    bool IsModalWindowOpen();
    void UpdateTombstoneWindow();
}