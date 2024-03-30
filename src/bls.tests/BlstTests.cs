using chia.dotnet.bls;
using supranational;
using System.Text;

namespace bls.tests;

public class BlstTests
{
    [Fact]
    public void LoadDll()
    {
        var msg = Encoding.UTF8.GetBytes("assertion");
        var DST = "MY-DST";

        var SK = new blst.SecretKey();
        var seed = Encoding.UTF8.GetBytes(new string('*', 32));
        SK.keygen(seed);

        var seed1 = new string('*', 32).HexStringToByteArray();
        var pk = PrivateKey.FromSeed(seed1);
        var key = pk.ToBytes();

        // generate public key and serialize it...
        var pk_for_wire = new blst.P1(SK).serialize();

        // sign |msg| and serialize the signature...
        var sig_for_wire = new blst.P2().hash_to(msg, DST, pk_for_wire)
                                        .sign_with(SK)
                                        .serialize();

        // now on "receiving" side, start with deserialization...
        var _sig = new blst.P2_Affine(sig_for_wire);
        var _pk = new blst.P1_Affine(pk_for_wire);
        if (!_pk.in_group())
            throw new blst.Exception(blst.ERROR.POINT_NOT_IN_GROUP);
        var ctx = new blst.Pairing(true, DST);
        var err = ctx.aggregate(_pk, _sig, msg, pk_for_wire);
        if (err != blst.ERROR.SUCCESS)
            throw new blst.Exception(err);
        ctx.commit();
        if (!ctx.finalverify())
            throw new blst.Exception(blst.ERROR.VERIFY_FAIL);
        Console.WriteLine("OK");

        // exercise .as_fp12 by performing equivalent of ctx.finalverify above
        var C1 = new blst.PT(_sig);
        var C2 = ctx.as_fp12();
        if (!blst.PT.finalverify(C1, C2))
            throw new blst.Exception(blst.ERROR.VERIFY_FAIL);

        // test integers as scalar multiplicands
        var p = blst.G1();
        var q = p.dup().dbl().dbl().add(p);
        if (!p.mult(5).is_equal(q))
            throw new ApplicationException("disaster");
        if (!blst.G1().mult(-5).is_equal(q.neg()))
            throw new ApplicationException("disaster");

        // low-order sanity check
        var p11 = new blst.P1("80803f0d09fec09a95f2ee7495323c15c162270c7cceaffa8566e941c66bcf206e72955d58b3b32e564de3209d672ca5".HexStringToByteArray());
        if (p11.in_group())
            throw new ApplicationException("disaster");
        if (!p11.mult(11).is_inf())
            throw new ApplicationException("disaster");
    }
}