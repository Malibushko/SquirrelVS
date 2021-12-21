#include <memory_resource> 

template<size_t Size>
struct MemoryPool
{
  unsigned char Buffer[Size];

  std::pmr::monotonic_buffer_resource Pool{
        std::data(Buffer),
        std::size(Buffer),
        std::pmr::null_memory_resource()
  };
};