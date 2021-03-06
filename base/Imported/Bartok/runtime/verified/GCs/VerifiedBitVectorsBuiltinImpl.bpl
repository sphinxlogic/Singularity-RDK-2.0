//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

// This file contains proofs of the declarations in VerifiedBitVectorsBuiltin.bpl.
// Many proofs are totally automatic, so that the implementations below have
// empty bodies.  Other proofs require assertions to guide the prover.

// Verification requires the "/bv:z" option to enable Z3 support of bit vectors

// Imports:
//   - TrustedBitVectorsBuiltin.bpl
// Exports:
//   - VerifiedBitVectorsBuiltin.bpl

// \Spec#\bin\Boogie.exe /bv:z /noinfer TrustedBitVectorsBuiltin.bpl VerifiedBitVectorsBuiltin.bpl VerifiedBitVectorsBuiltinImpl.bpl

implementation $AndAligned(x:bv32)
{
}

implementation $AddAligned(x:bv32, y:bv32)
{
}

implementation $SubAligned(x:bv32, y:bv32)
{
}

implementation $NotAligned(b:bv32)
{
}

implementation $Initialize($unitSize:bv32)
{
}

implementation _BB4Zero(a:[int]int, off:int, aBase:int, bb:[int]int, i0:int, i1:int, i2:int, g1:int, g2:int, idx:int)
{
  assert $mul(128bv32, $shr(B(i2 - i0), 7bv32)) == B(i2 - i0);
  assert idx - g1 == 4 * I($shr(B(i2 - i0), 7bv32));
  assert (forall i:int::{TV(i)} TV(i) && i2 <= i && i < i2 + 128 ==> $shr(B(i - i0), 7bv32) == $shr(B(i2 - i0), 7bv32));
}

implementation _BB4GetBit(i0:int, k:int)
{
}

implementation _BB4SetBit(a:[int]int, on:int, off:int, aBase:int, bb:[int]int, i0:int, i1:int, i2:int, k:int, idx:int, bbb:int, ret:[int]int, g1:int, g2:int)
{
}

implementation _BB4Zero2(a:[int]int, aBase:int, bb:[int]int, i0:int, i1:int, i2:int, g1:int, g2:int, idx:int)
{
  assert $mul(64bv32, $shr(B(i2 - i0), 6bv32)) == B(i2 - i0);
  assert idx - g1 == 4 * I($shr(B(i2 - i0), 6bv32));
  assert (forall i:int::{TV(i)} TV(i) && i2 <= i && i < i2 + 64 ==> $shr(B(i - i0), 6bv32) == $shr(B(i2 - i0), 6bv32));
}

implementation _BB4Get2Bit(i0:int, k:int)
{
}

implementation _BB4Set2Bit(a:[int]int, val:int, aBase:int, bb:[int]int, i0:int, i1:int, i2:int, k:int, idx:int, bbb:int, _bbb:int, ret:[int]int, g1:int, g2:int)
{
}

implementation $Const()
{
}


