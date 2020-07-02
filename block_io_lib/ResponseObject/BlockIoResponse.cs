
namespace block_io_lib
{
    public class BlockIoResponse <T>
    {
        public string Status { get; set; }
        public T Data { get; set; }
    }
}
