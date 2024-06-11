using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{
    public interface IDecodetor
    {
       byte[] encode_packet(byte[] data);
        byte[] decode_packet(ref List<byte> cache);

        byte[] encode_obj(object data);
        object decode_obj(byte[] data);
        byte[] encode_msg(object data);
        object decode_msg(byte[] data);
    }
}
