﻿var _0xebed = ["\x67\x65\x74\x54\x69\x6D\x65", "\x5A\x5A\x5A\x5A\x5A", "\x3F\x48\x58\x3F", "\x3F\x48", "\x54\x59\x41\x45\x45\x2D\x48\x57\x45\x42\x5A\x2D\x53\x55\x49\x5A\x5A\x2D\x49\x58\x4A\x55\x46\x2D\x48\x57\x4B\x41\x41\x2D\x50\x4D\x4E\x47\x56\x2D\x4B\x4E\x55\x49\x44\x2D\x57\x42\x5A\x51\x54\x2D\x52\x41\x53\x52\x41\x2D\x47\x49\x4A\x45\x43\x2D\x41\x55\x4D\x5A\x4D\x2D\x4E\x59\x53\x42\x4D", "", "\x6C\x65\x6E\x67\x74\x68", "\x63\x68\x61\x72\x43\x6F\x64\x65\x41\x74", "\x66\x72\x6F\x6D\x43\x68\x61\x72\x43\x6F\x64\x65", "\x43\x42\x43", "\x64\x65\x73\x63\x72\x69\x70\x74\x69\x6F\x6E", "\x64\x65\x63\x72\x79\x70\x74\x69\x6F\x6E", "\x30", "\x30\x78", "\x69\x6E\x64\x65\x78\x4F\x66", "\x30\x58", "\x73\x75\x62\x73\x74\x72\x69\x6E\x67", "\x66\x6C\x6F\x6F\x72", "\x73\x6C\x69\x63\x65", "\x67\x65\x74\x4D\x69\x6C\x6C\x69\x73\x65\x63\x6F\x6E\x64\x73", "\x42\x6C\x6F\x6F\x69\x65\x21\x20\x20\x45\x6E\x74\x72\x6F\x70\x79\x20\x76\x65\x63\x74\x6F\x72\x20\x76\x6F\x69\x64\x20\x61\x74\x20\x63\x61\x6C\x6C\x20\x74\x6F\x20\x6B\x65\x79\x46\x72\x6F\x6D\x45\x6E\x74\x72\x6F\x70\x79\x2E", "\x6F\x62\x6A\x65\x63\x74", "\x6E\x75\x6D\x62\x65\x72", "\x73\x74\x72\x69\x6E\x67", "\x73\x70\x6C\x69\x74", "\x63\x6F\x6E\x63\x61\x74", "\x6E\x65\x78\x74\x49\x6E\x74", "\x45\x43\x42", "\x0A", "\x20", "\x41", "\x5A", "\x59\x59\x59\x59\x59", "\x65\x6E\x63\x72\x79\x70\x74", "\x64\x65\x63\x72\x79\x70\x74", "\x6B\x65\x79", "\x69\x74\x65\x78\x74", "\x39\x46\x34\x38\x39\x36\x31\x33\x32\x34\x38\x31\x34\x38\x46\x39\x43\x32\x37\x39\x34\x35\x43\x36\x41\x45\x36\x32\x45\x45\x43\x41\x33\x45\x33\x33\x36\x37\x42\x42\x31\x34\x30\x36\x34\x45\x34\x45\x36\x44\x43\x36\x37\x41\x39\x46\x32\x38\x41\x42\x33\x42\x44\x31", "\x6E\x62\x79\x74\x65\x73", "\x6E\x65\x78\x74", "\x6E\x65\x78\x74\x62\x69\x74\x73", "\x72\x6F\x75\x6E\x64", "\x67\x65\x6E\x31", "\x67\x65\x6E\x32", "\x73\x74\x61\x74\x65\x45\x6E", "\x73\x68\x75\x66\x66\x6C\x65"]; var loadTime = (new Date())[_0xebed[0]](); var key; var prng; var entropyData = new Array(); var edlen = 0; var codegroupSentinel = _0xebed[1]; var keySizeInBits = 256; var blockSizeInBits = 128; var maxLineLength = 64; var hexSentinel = _0xebed[2], hexEndSentinel = _0xebed[3]; var licensekey = _0xebed[4]; var roundsArray = [, , , , [, , , , 10, , 12, , 14], , [, , , , 12, , 12, , 14], , [, , , , 14, , 14, , 14]]; var shiftOffsets = [, , , , [, 1, 2, 3], , [, 1, 2, 3], , [, 1, 3, 4]]; var RconEn = [0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91]; var SBoxEn = [99, 124, 119, 123, 242, 107, 111, 197, 48, 1, 103, 43, 254, 215, 171, 118, 202, 130, 201, 125, 250, 89, 71, 240, 173, 212, 162, 175, 156, 164, 114, 192, 183, 253, 147, 38, 54, 63, 247, 204, 52, 165, 229, 241, 113, 216, 49, 21, 4, 199, 35, 195, 24, 150, 5, 154, 7, 18, 128, 226, 235, 39, 178, 117, 9, 131, 44, 26, 27, 110, 90, 160, 82, 59, 214, 179, 41, 227, 47, 132, 83, 209, 0, 237, 32, 252, 177, 91, 106, 203, 190, 57, 74, 76, 88, 207, 208, 239, 170, 251, 67, 77, 51, 133, 69, 249, 2, 127, 80, 60, 159, 168, 81, 163, 64, 143, 146, 157, 56, 245, 188, 182, 218, 33, 16, 255, 243, 210, 205, 12, 19, 236, 95, 151, 68, 23, 196, 167, 126, 61, 100, 93, 25, 115, 96, 129, 79, 220, 34, 42, 144, 136, 70, 238, 184, 20, 222, 94, 11, 219, 224, 50, 58, 10, 73, 6, 36, 92, 194, 211, 172, 98, 145, 149, 228, 121, 231, 200, 55, 109, 141, 213, 78, 169, 108, 86, 244, 234, 101, 122, 174, 8, 186, 120, 37, 46, 28, 166, 180, 198, 232, 221, 116, 31, 75, 189, 139, 138, 112, 62, 181, 102, 72, 3, 246, 14, 97, 53, 87, 185, 134, 193, 29, 158, 225, 248, 152, 17, 105, 217, 142, 148, 155, 30, 135, 233, 206, 85, 40, 223, 140, 161, 137, 13, 191, 230, 66, 104, 65, 153, 45, 15, 176, 84, 187, 22]; var SBoxEnInverse = [82, 9, 106, 213, 48, 54, 165, 56, 191, 64, 163, 158, 129, 243, 215, 251, 124, 227, 57, 130, 155, 47, 255, 135, 52, 142, 67, 68, 196, 222, 233, 203, 84, 123, 148, 50, 166, 194, 35, 61, 238, 76, 149, 11, 66, 250, 195, 78, 8, 46, 161, 102, 40, 217, 36, 178, 118, 91, 162, 73, 109, 139, 209, 37, 114, 248, 246, 100, 134, 104, 152, 22, 212, 164, 92, 204, 93, 101, 182, 146, 108, 112, 72, 80, 253, 237, 185, 218, 94, 21, 70, 87, 167, 141, 157, 132, 144, 216, 171, 0, 140, 188, 211, 10, 247, 228, 88, 5, 184, 179, 69, 6, 208, 44, 30, 143, 202, 63, 15, 2, 193, 175, 189, 3, 1, 19, 138, 107, 58, 145, 17, 65, 79, 103, 220, 234, 151, 242, 207, 206, 240, 180, 230, 115, 150, 172, 116, 34, 231, 173, 53, 133, 226, 249, 55, 232, 28, 117, 223, 110, 71, 241, 26, 113, 29, 41, 197, 137, 111, 183, 98, 14, 170, 24, 190, 27, 252, 86, 62, 75, 198, 210, 121, 32, 154, 219, 192, 254, 120, 205, 90, 244, 31, 221, 168, 51, 136, 7, 199, 49, 177, 18, 16, 89, 39, 128, 236, 95, 96, 81, 127, 169, 25, 181, 74, 13, 45, 229, 122, 159, 147, 201, 156, 239, 160, 224, 59, 77, 174, 42, 245, 176, 200, 235, 187, 60, 131, 83, 153, 97, 23, 43, 4, 126, 186, 119, 214, 38, 225, 105, 20, 99, 85, 33, 12, 125]; var Nk = keySizeInBits / 32; var Nb = blockSizeInBits / 32; var Nr = roundsArray[Nk][Nb]; var licenseDetails = null; var stateEn = new array(4); var countEn = new array(2); countEn[0] = 0; countEn[1] = 0; var bufferEn = new array(64); var transformbufferEn = new array(16); var digestBitsEn = new array(16); var S11 = 7; var S12 = 12; var S13 = 17; var S14 = 22; var S21 = 5; var S22 = 9; var S23 = 14; var S24 = 20; var S31 = 4; var S32 = 11; var S33 = 16; var S34 = 23; var S41 = 6; var S42 = 10; var S43 = 15; var S44 = 21; function EncryptKey(licenseDetails) { var _0xae19x2c; var _0xae19x2d, _0xae19x2e; var _0xae19x2f; var _0xae19x30 = _0xebed[5]; try { setKey(); addEntropyTime(); prng = new AESprng(keyFromEntropy()); _0xae19x2c = encode_utf8(licenseDetails); md5_init(); for (_0xae19x2e = 0; _0xae19x2e < _0xae19x2c[_0xebed[6]]; _0xae19x2e++) { md5_update(_0xae19x2c[_0xebed[7]](_0xae19x2e)) }; md5_finish(); for (_0xae19x2e = 0; _0xae19x2e < digestBitsEn[_0xebed[6]]; _0xae19x2e++) { _0xae19x30 += String[_0xebed[8]](digestBitsEn[_0xae19x2e]) }; _0xae19x2e = _0xae19x2c[_0xebed[6]]; _0xae19x30 += String[_0xebed[8]](_0xae19x2e >>> 24); _0xae19x30 += String[_0xebed[8]](_0xae19x2e >>> 16); _0xae19x30 += String[_0xebed[8]](_0xae19x2e >>> 8); _0xae19x30 += String[_0xebed[8]](_0xae19x2e & 0xFF); _0xae19x2f = rijndaelEncrypt(_0xae19x30 + _0xae19x2c, key, _0xebed[9]); _0xae19x2d = armour_codegroup(_0xae19x2f); delete prng } catch (e) { alert(e[_0xebed[10]]) }; return _0xae19x2d } function setKey() { var _0xae19x32 = encode_utf8(licensekey); var _0xae19x2e, _0xae19x33, _0xae19x34, _0xae19x35; try { if (_0xae19x32[_0xebed[6]] == 1) { _0xae19x32 += _0xae19x32 }; md5_init(); for (_0xae19x2e = 0; _0xae19x2e < _0xae19x32[_0xebed[6]]; _0xae19x2e += 2) { md5_update(_0xae19x32[_0xebed[7]](_0xae19x2e)) }; md5_finish(); _0xae19x33 = byteArrayToHex(digestBitsEn); md5_init(); for (_0xae19x2e = 1; _0xae19x2e < _0xae19x32[_0xebed[6]]; _0xae19x2e += 2) { md5_update(_0xae19x32[_0xebed[7]](_0xae19x2e)) }; md5_finish(); _0xae19x34 = byteArrayToHex(digestBitsEn); _0xae19x35 = _0xae19x33 + _0xae19x34; key = hexToByteArray(_0xae19x35); _0xae19x35 = byteArrayToHex(key) } catch (e) { alert(e[_0xebed[10]]) } } function encode_utf8(_0xae19x32) { var _0xae19x2e, _0xae19x37 = false; try { for (_0xae19x2e = 0; _0xae19x2e < _0xae19x32[_0xebed[6]]; _0xae19x2e++) { if ((_0xae19x32[_0xebed[7]](_0xae19x2e) == 0x9D) || (_0xae19x32[_0xebed[7]](_0xae19x2e) > 0xFF)) { _0xae19x37 = true; break } }; if (!_0xae19x37) { return _0xae19x32 } } catch (e) { alert(e[_0xebed[10]]) }; return String[_0xebed[8]](0x9D) + unicode_to_utf8(_0xae19x32) } function md5_init() { countEn[0] = countEn[1] = 0; stateEn[0] = 0x67452301; stateEn[1] = 0xefcdab89; stateEn[2] = 0x98badcfe; stateEn[3] = 0x10325476; for (i = 0; i < digestBitsEn[_0xebed[6]]; i++) { digestBitsEn[i] = 0 } } function md5_update(_0xae19x3a) { var _0xae19x3b, _0xae19x2e; _0xae19x3b = and(shr(countEn[0], 3), 0x3F); if (countEn[0] < 0xFFFFFFFF - 7) { countEn[0] += 8 } else { countEn[1]++; countEn[0] -= 0xFFFFFFFF + 1; countEn[0] += 8 }; bufferEn[_0xae19x3b] = and(_0xae19x3a, 0xff); if (_0xae19x3b >= 63) { transform(bufferEn, 0) } } function md5_finish() { var _0xae19x3d = new array(8); var _0xae19x3e; var _0xae19x2e = 0, _0xae19x3b = 0, _0xae19x3f = 0; try { for (_0xae19x2e = 0; _0xae19x2e < 4; _0xae19x2e++) { _0xae19x3d[_0xae19x2e] = and(shr(countEn[0], (_0xae19x2e * 8)), 0xFF) }; for (_0xae19x2e = 0; _0xae19x2e < 4; _0xae19x2e++) { _0xae19x3d[_0xae19x2e + 4] = and(shr(countEn[1], (_0xae19x2e * 8)), 0xFF) }; _0xae19x3b = and(shr(countEn[0], 3), 0x3F); _0xae19x3f = (_0xae19x3b < 56) ? (56 - _0xae19x3b) : (120 - _0xae19x3b); _0xae19x3e = new array(64); _0xae19x3e[0] = 0x80; for (_0xae19x2e = 0; _0xae19x2e < _0xae19x3f; _0xae19x2e++) { md5_update(_0xae19x3e[_0xae19x2e]) }; for (_0xae19x2e = 0; _0xae19x2e < 8; _0xae19x2e++) { md5_update(_0xae19x3d[_0xae19x2e]) }; for (_0xae19x2e = 0; _0xae19x2e < 4; _0xae19x2e++) { for (j = 0; j < 4; j++) { digestBitsEn[_0xae19x2e * 4 + j] = and(shr(stateEn[_0xae19x2e], (j * 8)), 0xFF) } } } catch (e) { alert(e[_0xebed[11]]) } } function byteArrayToHex(_0xae19x41) { var _0xae19x42 = _0xebed[5]; if (!_0xae19x41) { return }; for (var _0xae19x2e = 0; _0xae19x2e < _0xae19x41[_0xebed[6]]; _0xae19x2e++) { _0xae19x42 += ((_0xae19x41[_0xae19x2e] < 16) ? _0xebed[12] : _0xebed[5]) + _0xae19x41[_0xae19x2e].toString(16) }; return _0xae19x42 } function hexToByteArray(_0xae19x44) { var _0xae19x41 = []; if (_0xae19x44[_0xebed[6]] % 2) { return }; if (_0xae19x44[_0xebed[14]](_0xebed[13]) == 0 || _0xae19x44[_0xebed[14]](_0xebed[15]) == 0) { _0xae19x44 = _0xae19x44[_0xebed[16]](2) }; for (var _0xae19x2e = 0; _0xae19x2e < _0xae19x44[_0xebed[6]]; _0xae19x2e += 2) { _0xae19x41[Math[_0xebed[17]](_0xae19x2e / 2)] = parseInt(_0xae19x44[_0xebed[18]](_0xae19x2e, _0xae19x2e + 2), 16) }; return _0xae19x41 } function array(_0xae19x46) { for (i = 0; i < _0xae19x46; i++) { this[i] = 0 }; this[_0xebed[6]] = _0xae19x46 } function integer(_0xae19x46) { return _0xae19x46 % (0xffffffff + 1) } function shr(_0xae19x49, _0xae19x3a) { _0xae19x49 = integer(_0xae19x49); _0xae19x3a = integer(_0xae19x3a); if (_0xae19x49 - 0x80000000 >= 0) { _0xae19x49 = _0xae19x49 % 0x80000000; _0xae19x49 >>= _0xae19x3a; _0xae19x49 += 0x40000000 >> (_0xae19x3a - 1) } else { _0xae19x49 >>= _0xae19x3a }; return _0xae19x49 } function shl1(_0xae19x49) { _0xae19x49 = _0xae19x49 % 0x80000000; if (_0xae19x49 & 0x40000000 == 0x40000000) { _0xae19x49 -= 0x40000000; _0xae19x49 *= 2; _0xae19x49 += 0x80000000 } else { _0xae19x49 *= 2 }; return _0xae19x49 } function shl(_0xae19x49, _0xae19x3a) { _0xae19x49 = integer(_0xae19x49); _0xae19x3a = integer(_0xae19x3a); for (var _0xae19x2e = 0; _0xae19x2e < _0xae19x3a; _0xae19x2e++) { _0xae19x49 = shl1(_0xae19x49) }; return _0xae19x49 } function and(_0xae19x49, _0xae19x3a) { _0xae19x49 = integer(_0xae19x49); _0xae19x3a = integer(_0xae19x3a); var _0xae19x4d = _0xae19x49 - 0x80000000; var _0xae19x4e = _0xae19x3a - 0x80000000; if (_0xae19x4d >= 0) { if (_0xae19x4e >= 0) { return ((_0xae19x4d & _0xae19x4e) + 0x80000000) } else { return (_0xae19x4d & _0xae19x3a) } } else { if (_0xae19x4e >= 0) { return (_0xae19x49 & _0xae19x4e) } else { return (_0xae19x49 & _0xae19x3a) } } } function or(_0xae19x49, _0xae19x3a) { _0xae19x49 = integer(_0xae19x49); _0xae19x3a = integer(_0xae19x3a); var _0xae19x4d = _0xae19x49 - 0x80000000; var _0xae19x4e = _0xae19x3a - 0x80000000; if (_0xae19x4d >= 0) { if (_0xae19x4e >= 0) { return ((_0xae19x4d | _0xae19x4e) + 0x80000000) } else { return ((_0xae19x4d | _0xae19x3a) + 0x80000000) } } else { if (_0xae19x4e >= 0) { return ((_0xae19x49 | _0xae19x4e) + 0x80000000) } else { return (_0xae19x49 | _0xae19x3a) } } } function xor(_0xae19x49, _0xae19x3a) { _0xae19x49 = integer(_0xae19x49); _0xae19x3a = integer(_0xae19x3a); var _0xae19x4d = _0xae19x49 - 0x80000000; var _0xae19x4e = _0xae19x3a - 0x80000000; if (_0xae19x4d >= 0) { if (_0xae19x4e >= 0) { return (_0xae19x4d ^ _0xae19x4e) } else { return ((_0xae19x4d ^ _0xae19x3a) + 0x80000000) } } else { if (_0xae19x4e >= 0) { return ((_0xae19x49 ^ _0xae19x4e) + 0x80000000) } else { return (_0xae19x49 ^ _0xae19x3a) } } } function not(_0xae19x49) { _0xae19x49 = integer(_0xae19x49); return 0xffffffff - _0xae19x49 } function F(_0xae19x53, _0xae19x54, _0xae19x55) { return or(and(_0xae19x53, _0xae19x54), and(not(_0xae19x53), _0xae19x55)) } function G(_0xae19x53, _0xae19x54, _0xae19x55) { return or(and(_0xae19x53, _0xae19x55), and(_0xae19x54, not(_0xae19x55))) } function H(_0xae19x53, _0xae19x54, _0xae19x55) { return xor(xor(_0xae19x53, _0xae19x54), _0xae19x55) } function I(_0xae19x53, _0xae19x54, _0xae19x55) { return xor(_0xae19x54, or(_0xae19x53, not(_0xae19x55))) } function rotateLeft(_0xae19x49, _0xae19x46) { return or(shl(_0xae19x49, _0xae19x46), (shr(_0xae19x49, (32 - _0xae19x46)))) } function FF(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53, _0xae19x32, _0xae19x5d) { _0xae19x49 = _0xae19x49 + F(_0xae19x3a, _0xae19x5b, _0xae19x5c) + _0xae19x53 + _0xae19x5d; _0xae19x49 = rotateLeft(_0xae19x49, _0xae19x32); _0xae19x49 = _0xae19x49 + _0xae19x3a; return _0xae19x49 } function GG(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53, _0xae19x32, _0xae19x5d) { _0xae19x49 = _0xae19x49 + G(_0xae19x3a, _0xae19x5b, _0xae19x5c) + _0xae19x53 + _0xae19x5d; _0xae19x49 = rotateLeft(_0xae19x49, _0xae19x32); _0xae19x49 = _0xae19x49 + _0xae19x3a; return _0xae19x49 } function HH(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53, _0xae19x32, _0xae19x5d) { _0xae19x49 = _0xae19x49 + H(_0xae19x3a, _0xae19x5b, _0xae19x5c) + _0xae19x53 + _0xae19x5d; _0xae19x49 = rotateLeft(_0xae19x49, _0xae19x32); _0xae19x49 = _0xae19x49 + _0xae19x3a; return _0xae19x49 } function II(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53, _0xae19x32, _0xae19x5d) { _0xae19x49 = _0xae19x49 + I(_0xae19x3a, _0xae19x5b, _0xae19x5c) + _0xae19x53 + _0xae19x5d; _0xae19x49 = rotateLeft(_0xae19x49, _0xae19x32); _0xae19x49 = _0xae19x49 + _0xae19x3a; return _0xae19x49 } function transform(_0xae19x62, _0xae19x63) { var _0xae19x49 = 0, _0xae19x3a = 0, _0xae19x5b = 0, _0xae19x5c = 0; var _0xae19x53 = transformbufferEn; _0xae19x49 = stateEn[0]; _0xae19x3a = stateEn[1]; _0xae19x5b = stateEn[2]; _0xae19x5c = stateEn[3]; for (i = 0; i < 16; i++) { _0xae19x53[i] = and(_0xae19x62[i * 4 + _0xae19x63], 0xFF); for (j = 1; j < 4; j++) { _0xae19x53[i] += shl(and(_0xae19x62[i * 4 + j + _0xae19x63], 0xFF), j * 8) } }; _0xae19x49 = FF(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[0], S11, 0xd76aa478); _0xae19x5c = FF(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[1], S12, 0xe8c7b756); _0xae19x5b = FF(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[2], S13, 0x242070db); _0xae19x3a = FF(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[3], S14, 0xc1bdceee); _0xae19x49 = FF(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[4], S11, 0xf57c0faf); _0xae19x5c = FF(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[5], S12, 0x4787c62a); _0xae19x5b = FF(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[6], S13, 0xa8304613); _0xae19x3a = FF(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[7], S14, 0xfd469501); _0xae19x49 = FF(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[8], S11, 0x698098d8); _0xae19x5c = FF(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[9], S12, 0x8b44f7af); _0xae19x5b = FF(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[10], S13, 0xffff5bb1); _0xae19x3a = FF(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[11], S14, 0x895cd7be); _0xae19x49 = FF(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[12], S11, 0x6b901122); _0xae19x5c = FF(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[13], S12, 0xfd987193); _0xae19x5b = FF(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[14], S13, 0xa679438e); _0xae19x3a = FF(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[15], S14, 0x49b40821); _0xae19x49 = GG(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[1], S21, 0xf61e2562); _0xae19x5c = GG(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[6], S22, 0xc040b340); _0xae19x5b = GG(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[11], S23, 0x265e5a51); _0xae19x3a = GG(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[0], S24, 0xe9b6c7aa); _0xae19x49 = GG(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[5], S21, 0xd62f105d); _0xae19x5c = GG(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[10], S22, 0x2441453); _0xae19x5b = GG(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[15], S23, 0xd8a1e681); _0xae19x3a = GG(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[4], S24, 0xe7d3fbc8); _0xae19x49 = GG(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[9], S21, 0x21e1cde6); _0xae19x5c = GG(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[14], S22, 0xc33707d6); _0xae19x5b = GG(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[3], S23, 0xf4d50d87); _0xae19x3a = GG(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[8], S24, 0x455a14ed); _0xae19x49 = GG(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[13], S21, 0xa9e3e905); _0xae19x5c = GG(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[2], S22, 0xfcefa3f8); _0xae19x5b = GG(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[7], S23, 0x676f02d9); _0xae19x3a = GG(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[12], S24, 0x8d2a4c8a); _0xae19x49 = HH(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[5], S31, 0xfffa3942); _0xae19x5c = HH(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[8], S32, 0x8771f681); _0xae19x5b = HH(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[11], S33, 0x6d9d6122); _0xae19x3a = HH(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[14], S34, 0xfde5380c); _0xae19x49 = HH(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[1], S31, 0xa4beea44); _0xae19x5c = HH(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[4], S32, 0x4bdecfa9); _0xae19x5b = HH(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[7], S33, 0xf6bb4b60); _0xae19x3a = HH(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[10], S34, 0xbebfbc70); _0xae19x49 = HH(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[13], S31, 0x289b7ec6); _0xae19x5c = HH(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[0], S32, 0xeaa127fa); _0xae19x5b = HH(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[3], S33, 0xd4ef3085); _0xae19x3a = HH(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[6], S34, 0x4881d05); _0xae19x49 = HH(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[9], S31, 0xd9d4d039); _0xae19x5c = HH(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[12], S32, 0xe6db99e5); _0xae19x5b = HH(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[15], S33, 0x1fa27cf8); _0xae19x3a = HH(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[2], S34, 0xc4ac5665); _0xae19x49 = II(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[0], S41, 0xf4292244); _0xae19x5c = II(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[7], S42, 0x432aff97); _0xae19x5b = II(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[14], S43, 0xab9423a7); _0xae19x3a = II(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[5], S44, 0xfc93a039); _0xae19x49 = II(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[12], S41, 0x655b59c3); _0xae19x5c = II(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[3], S42, 0x8f0ccc92); _0xae19x5b = II(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[10], S43, 0xffeff47d); _0xae19x3a = II(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[1], S44, 0x85845dd1); _0xae19x49 = II(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[8], S41, 0x6fa87e4f); _0xae19x5c = II(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[15], S42, 0xfe2ce6e0); _0xae19x5b = II(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[6], S43, 0xa3014314); _0xae19x3a = II(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[13], S44, 0x4e0811a1); _0xae19x49 = II(_0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x53[4], S41, 0xf7537e82); _0xae19x5c = II(_0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x5b, _0xae19x53[11], S42, 0xbd3af235); _0xae19x5b = II(_0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x3a, _0xae19x53[2], S43, 0x2ad7d2bb); _0xae19x3a = II(_0xae19x3a, _0xae19x5b, _0xae19x5c, _0xae19x49, _0xae19x53[9], S44, 0xeb86d391); stateEn[0] += _0xae19x49; stateEn[1] += _0xae19x3a; stateEn[2] += _0xae19x5b; stateEn[3] += _0xae19x5c } function addEntropyByte(_0xae19x3a) { entropyData[edlen++] = _0xae19x3a } function ce() { addEntropyByte(Math[_0xebed[17]]((((new Date)[_0xebed[19]]()) * 255) / 999)) } function addEntropy32(_0xae19x67) { var _0xae19x2e; for (_0xae19x2e = 0; _0xae19x2e < 4; _0xae19x2e++) { addEntropyByte(_0xae19x67 & 0xFF); _0xae19x67 >>= 8 } } function addEntropyTime() { addEntropy32((new Date())[_0xebed[0]]()) } function keyFromEntropy() { var _0xae19x2e, _0xae19x6a = new Array(32); if (edlen == 0) { alert(_0xebed[20]) }; md5_init(); for (_0xae19x2e = 0; _0xae19x2e < edlen; _0xae19x2e += 2) { md5_update(entropyData[_0xae19x2e]) }; md5_finish(); for (_0xae19x2e = 0; _0xae19x2e < 16; _0xae19x2e++) { _0xae19x6a[_0xae19x2e] = digestBitsEn[_0xae19x2e] }; md5_init(); for (_0xae19x2e = 1; _0xae19x2e < edlen; _0xae19x2e += 2) { md5_update(entropyData[_0xae19x2e]) }; md5_finish(); for (_0xae19x2e = 0; _0xae19x2e < 16; _0xae19x2e++) { _0xae19x6a[_0xae19x2e + 16] = digestBitsEn[_0xae19x2e] }; return _0xae19x6a } function encode_utf8(_0xae19x32) { var _0xae19x2e, _0xae19x37 = false; for (_0xae19x2e = 0; _0xae19x2e < _0xae19x32[_0xebed[6]]; _0xae19x2e++) { if ((_0xae19x32[_0xebed[7]](_0xae19x2e) == 0x9D) || (_0xae19x32[_0xebed[7]](_0xae19x2e) > 0xFF)) { _0xae19x37 = true; break } }; if (!_0xae19x37) { return _0xae19x32 }; return String[_0xebed[8]](0x9D) + unicode_to_utf8(_0xae19x32) } function decode_utf8(_0xae19x32) { if ((_0xae19x32[_0xebed[6]] > 0) && (_0xae19x32[_0xebed[7]](0) == 0x9D)) { return utf8_to_unicode(_0xae19x32[_0xebed[16]](1)) }; return _0xae19x32 } function packBytes(_0xae19x6d) { var stateEn = new Array(); if (!_0xae19x6d || _0xae19x6d[_0xebed[6]] % 4) { return }; stateEn[0] = new Array(); stateEn[1] = new Array(); stateEn[2] = new Array(); stateEn[3] = new Array(); for (var _0xae19x6e = 0; _0xae19x6e < _0xae19x6d[_0xebed[6]]; _0xae19x6e += 4) { stateEn[0][_0xae19x6e / 4] = _0xae19x6d[_0xae19x6e]; stateEn[1][_0xae19x6e / 4] = _0xae19x6d[_0xae19x6e + 1]; stateEn[2][_0xae19x6e / 4] = _0xae19x6d[_0xae19x6e + 2]; stateEn[3][_0xae19x6e / 4] = _0xae19x6d[_0xae19x6e + 3] }; return stateEn } function unpackBytes(_0xae19x70) { var _0xae19x42 = new Array(); for (var _0xae19x6e = 0; _0xae19x6e < _0xae19x70[0][_0xebed[6]]; _0xae19x6e++) { _0xae19x42[_0xae19x42[_0xebed[6]]] = _0xae19x70[0][_0xae19x6e]; _0xae19x42[_0xae19x42[_0xebed[6]]] = _0xae19x70[1][_0xae19x6e]; _0xae19x42[_0xae19x42[_0xebed[6]]] = _0xae19x70[2][_0xae19x6e]; _0xae19x42[_0xae19x42[_0xebed[6]]] = _0xae19x70[3][_0xae19x6e] }; return _0xae19x42 } function formatPlaintext(_0xae19x2c) { var _0xae19x72 = blockSizeInBits / 8; var _0xae19x2e; if ((!((typeof _0xae19x2c == _0xebed[21]) && ((typeof (_0xae19x2c[0])) == _0xebed[22]))) && ((typeof _0xae19x2c == _0xebed[23]) || _0xae19x2c[_0xebed[14]])) { _0xae19x2c = _0xae19x2c[_0xebed[24]](_0xebed[5]); for (_0xae19x2e = 0; _0xae19x2e < _0xae19x2c[_0xebed[6]]; _0xae19x2e++) { _0xae19x2c[_0xae19x2e] = _0xae19x2c[_0xae19x2e][_0xebed[7]](0) & 0xFF } }; _0xae19x2e = _0xae19x2c[_0xebed[6]] % _0xae19x72; if (_0xae19x2e > 0) { _0xae19x2c = _0xae19x2c[_0xebed[25]](getRandomBytes(_0xae19x72 - _0xae19x2e)) }; return _0xae19x2c } function getRandomBytes(_0xae19x74) { var _0xae19x2e, _0xae19x75 = new Array(); for (_0xae19x2e = 0; _0xae19x2e < _0xae19x74; _0xae19x2e++) { _0xae19x75[_0xae19x2e] = prng[_0xebed[26]](255) }; return _0xae19x75 } function rijndaelEncrypt(_0xae19x2c, key, _0xae19x77) { var _0xae19x78, _0xae19x2e, _0xae19x79; var _0xae19x72 = blockSizeInBits / 8; var _0xae19x7a; if (!_0xae19x2c || !key) { return }; if (key[_0xebed[6]] * 8 != keySizeInBits) { return }; if (_0xae19x77 == _0xebed[9]) { _0xae19x7a = getRandomBytes(_0xae19x72) } else { _0xae19x77 = _0xebed[27]; _0xae19x7a = new Array() }; _0xae19x2c = formatPlaintext(_0xae19x2c); _0xae19x78 = keyExpansion(key); for (var _0xae19x7b = 0; _0xae19x7b < _0xae19x2c[_0xebed[6]] / _0xae19x72; _0xae19x7b++) { _0xae19x79 = _0xae19x2c[_0xebed[18]](_0xae19x7b * _0xae19x72, (_0xae19x7b + 1) * _0xae19x72); if (_0xae19x77 == _0xebed[9]) { for (var _0xae19x2e = 0; _0xae19x2e < _0xae19x72; _0xae19x2e++) { _0xae19x79[_0xae19x2e] ^= _0xae19x7a[(_0xae19x7b * _0xae19x72) + _0xae19x2e] } }; _0xae19x7a = _0xae19x7a[_0xebed[25]](encrypt(_0xae19x79, _0xae19x78)) }; return _0xae19x7a } var acgcl, acgt, acgg; function armour_cg_outgroup() { if (acgcl[_0xebed[6]] > maxLineLength) { acgt += acgcl + _0xebed[28]; acgcl = _0xebed[5] }; if (acgcl[_0xebed[6]] > 0) { acgcl += _0xebed[29] }; acgcl += acgg; acgg = _0xebed[5] } function armour_cg_outletter(_0xae19x81) { if (acgg[_0xebed[6]] >= 5) { armour_cg_outgroup() }; acgg += _0xae19x81 } function armour_codegroup(_0xae19x3a) { var _0xae19x83 = (_0xebed[30])[_0xebed[7]](0); acgcl = codegroupSentinel; acgt = _0xebed[5]; acgg = _0xebed[5]; var _0xae19x84 = new LEcuyer(0xbadf00d); for (i = 0; i < _0xae19x3a[_0xebed[6]]; i++) { var _0xae19x85 = _0xae19x84[_0xebed[26]](23); armour_cg_outletter(String[_0xebed[8]](_0xae19x83 + ((((_0xae19x3a[i] >> 4) & 0xF)) + _0xae19x85) % 24)); _0xae19x85 = _0xae19x84[_0xebed[26]](23); armour_cg_outletter(String[_0xebed[8]](_0xae19x83 + ((((_0xae19x3a[i] & 0xF)) + _0xae19x85) % 24))) }; delete _0xae19x84; while (acgg[_0xebed[6]] < 5) { armour_cg_outletter(_0xebed[31]) }; armour_cg_outgroup(); acgg = _0xebed[32]; armour_cg_outgroup(); acgt += acgcl + _0xebed[28]; return acgt } function cyclicShiftLeft(_0xae19x87, _0xae19x88) { var _0xae19x89 = _0xae19x87[_0xebed[18]](0, _0xae19x88); _0xae19x87 = _0xae19x87[_0xebed[18]](_0xae19x88)[_0xebed[25]](_0xae19x89); return _0xae19x87 } function xtime(_0xae19x8b) { _0xae19x8b <<= 1; return ((_0xae19x8b & 0x100) ? (_0xae19x8b ^ 0x11B) : (_0xae19x8b)) } function mult_GF256(_0xae19x53, _0xae19x54) { var _0xae19x8d, _0xae19x42 = 0; for (_0xae19x8d = 1; _0xae19x8d < 256; _0xae19x8d *= 2, _0xae19x54 = xtime(_0xae19x54)) { var _0xae19x8e = _0xae19x53 & _0xae19x8d; if (_0xae19x8e) { _0xae19x42 ^= _0xae19x54 } }; return _0xae19x42 } function byteSub(stateEn, _0xae19x90) { var _0xae19x91; if (_0xae19x90 == _0xebed[33]) { _0xae19x91 = SBoxEn } else { _0xae19x91 = SBoxEnInverse }; for (var _0xae19x2e = 0; _0xae19x2e < 4; _0xae19x2e++) { for (var _0xae19x6e = 0; _0xae19x6e < Nb; _0xae19x6e++) { stateEn[_0xae19x2e][_0xae19x6e] = _0xae19x91[stateEn[_0xae19x2e][_0xae19x6e]] } } } function shiftRow(stateEn, _0xae19x90) { for (var _0xae19x2e = 1; _0xae19x2e < 4; _0xae19x2e++) { if (_0xae19x90 == _0xebed[33]) { stateEn[_0xae19x2e] = cyclicShiftLeft(stateEn[_0xae19x2e], shiftOffsets[Nb][_0xae19x2e]) } else { stateEn[_0xae19x2e] = cyclicShiftLeft(stateEn[_0xae19x2e], Nb - shiftOffsets[Nb][_0xae19x2e]) } } } function mixColumn(stateEn, _0xae19x90) { var _0xae19x3a = []; for (var _0xae19x6e = 0; _0xae19x6e < Nb; _0xae19x6e++) { for (var _0xae19x2e = 0; _0xae19x2e < 4; _0xae19x2e++) { if (_0xae19x90 == _0xebed[33]) { _0xae19x3a[_0xae19x2e] = mult_GF256(stateEn[_0xae19x2e][_0xae19x6e], 2) ^ mult_GF256(stateEn[(_0xae19x2e + 1) % 4][_0xae19x6e], 3) ^ stateEn[(_0xae19x2e + 2) % 4][_0xae19x6e] ^ stateEn[(_0xae19x2e + 3) % 4][_0xae19x6e] } else { _0xae19x3a[_0xae19x2e] = mult_GF256(stateEn[_0xae19x2e][_0xae19x6e], 0xE) ^ mult_GF256(stateEn[(_0xae19x2e + 1) % 4][_0xae19x6e], 0xB) ^ mult_GF256(stateEn[(_0xae19x2e + 2) % 4][_0xae19x6e], 0xD) ^ mult_GF256(stateEn[(_0xae19x2e + 3) % 4][_0xae19x6e], 9) } }; for (var _0xae19x2e = 0; _0xae19x2e < 4; _0xae19x2e++) { stateEn[_0xae19x2e][_0xae19x6e] = _0xae19x3a[_0xae19x2e] } } } function addRoundKey(stateEn, _0xae19x95) { for (var _0xae19x6e = 0; _0xae19x6e < Nb; _0xae19x6e++) { stateEn[0][_0xae19x6e] ^= (_0xae19x95[_0xae19x6e] & 0xFF); stateEn[1][_0xae19x6e] ^= ((_0xae19x95[_0xae19x6e] >> 8) & 0xFF); stateEn[2][_0xae19x6e] ^= ((_0xae19x95[_0xae19x6e] >> 16) & 0xFF); stateEn[3][_0xae19x6e] ^= ((_0xae19x95[_0xae19x6e] >> 24) & 0xFF) } } function keyExpansion(key) { var _0xae19x78 = new Array(); var _0xae19x89; Nk = keySizeInBits / 32; Nb = blockSizeInBits / 32; Nr = roundsArray[Nk][Nb]; for (var _0xae19x6e = 0; _0xae19x6e < Nk; _0xae19x6e++) { _0xae19x78[_0xae19x6e] = (key[4 * _0xae19x6e]) | (key[4 * _0xae19x6e + 1] << 8) | (key[4 * _0xae19x6e + 2] << 16) | (key[4 * _0xae19x6e + 3] << 24) }; for (_0xae19x6e = Nk; _0xae19x6e < Nb * (Nr + 1) ; _0xae19x6e++) { _0xae19x89 = _0xae19x78[_0xae19x6e - 1]; if (_0xae19x6e % Nk == 0) { _0xae19x89 = ((SBoxEn[(_0xae19x89 >> 8) & 0xFF]) | (SBoxEn[(_0xae19x89 >> 16) & 0xFF] << 8) | (SBoxEn[(_0xae19x89 >> 24) & 0xFF] << 16) | (SBoxEn[_0xae19x89 & 0xFF] << 24)) ^ RconEn[Math[_0xebed[17]](_0xae19x6e / Nk) - 1] } else { if (Nk > 6 && _0xae19x6e % Nk == 4) { _0xae19x89 = (SBoxEn[(_0xae19x89 >> 24) & 0xFF] << 24) | (SBoxEn[(_0xae19x89 >> 16) & 0xFF] << 16) | (SBoxEn[(_0xae19x89 >> 8) & 0xFF] << 8) | (SBoxEn[_0xae19x89 & 0xFF]) } }; _0xae19x78[_0xae19x6e] = _0xae19x78[_0xae19x6e - Nk] ^ _0xae19x89 }; return _0xae19x78 } function Round(stateEn, _0xae19x95) { byteSub(stateEn, _0xebed[33]); shiftRow(stateEn, _0xebed[33]); mixColumn(stateEn, _0xebed[33]); addRoundKey(stateEn, _0xae19x95) } function InverseRound(stateEn, _0xae19x95) { addRoundKey(stateEn, _0xae19x95); mixColumn(stateEn, _0xebed[34]); shiftRow(stateEn, _0xebed[34]); byteSub(stateEn, _0xebed[34]) } function FinalRound(stateEn, _0xae19x95) { byteSub(stateEn, _0xebed[33]); shiftRow(stateEn, _0xebed[33]); addRoundKey(stateEn, _0xae19x95) } function InverseFinalRound(stateEn, _0xae19x95) { addRoundKey(stateEn, _0xae19x95); shiftRow(stateEn, _0xebed[34]); byteSub(stateEn, _0xebed[34]) } function encrypt(_0xae19x7b, _0xae19x78) { var _0xae19x2e; if (!_0xae19x7b || _0xae19x7b[_0xebed[6]] * 8 != blockSizeInBits) { return }; if (!_0xae19x78) { return }; _0xae19x7b = packBytes(_0xae19x7b); addRoundKey(_0xae19x7b, _0xae19x78); for (_0xae19x2e = 1; _0xae19x2e < Nr; _0xae19x2e++) { Round(_0xae19x7b, _0xae19x78[_0xebed[18]](Nb * _0xae19x2e, Nb * (_0xae19x2e + 1))) }; FinalRound(_0xae19x7b, _0xae19x78[_0xebed[18]](Nb * Nr)); return unpackBytes(_0xae19x7b) } function AESprng(_0xae19x9d) { this[_0xebed[35]] = new Array(); this[_0xebed[35]] = _0xae19x9d; this[_0xebed[36]] = hexToByteArray(_0xebed[37]); this[_0xebed[38]] = 0; this[_0xebed[39]] = AESprng_next; this[_0xebed[40]] = AESprng_nextbits; this[_0xebed[26]] = AESprng_nextInt; this[_0xebed[41]] = AESprng_round; bsb = blockSizeInBits; blockSizeInBits = 256; var _0xae19x2e, _0xae19x7a; for (_0xae19x2e = 0; _0xae19x2e < 3; _0xae19x2e++) { this[_0xebed[35]] = rijndaelEncrypt(this[_0xebed[36]], this[_0xebed[35]], _0xebed[27]) }; var _0xae19x46 = 1 + (this[_0xebed[35]][3] & 2) + (this[_0xebed[35]][9] & 1); for (_0xae19x2e = 0; _0xae19x2e < _0xae19x46; _0xae19x2e++) { this[_0xebed[35]] = rijndaelEncrypt(this[_0xebed[36]], this[_0xebed[35]], _0xebed[27]) }; blockSizeInBits = bsb } function AESprng_round() { bsb = blockSizeInBits; blockSizeInBits = 256; this[_0xebed[35]] = rijndaelEncrypt(this[_0xebed[36]], this[_0xebed[35]], _0xebed[27]); this[_0xebed[38]] = 32; blockSizeInBits = bsb } function AESprng_next() { if (this[_0xebed[38]] <= 0) { this[_0xebed[41]]() }; return (this[_0xebed[35]][--this[_0xebed[38]]]) } function AESprng_nextbits(_0xae19x46) { var _0xae19x2e, _0xae19x67 = 0, _0xae19xa1 = Math[_0xebed[17]]((_0xae19x46 + 7) / 8); for (_0xae19x2e = 0; _0xae19x2e < _0xae19xa1; _0xae19x2e++) { _0xae19x67 = (_0xae19x67 << 8) | this[_0xebed[39]]() }; return _0xae19x67 & ((1 << _0xae19x46) - 1) } function AESprng_nextInt(_0xae19x46) { var _0xae19xa3 = 1, _0xae19xa4 = 0; while (_0xae19x46 >= _0xae19xa3) { _0xae19xa3 <<= 1; _0xae19xa4++ }; _0xae19xa3--; while (true) { var _0xae19xa5 = this[_0xebed[40]](_0xae19xa4) & _0xae19xa3; if (_0xae19xa5 <= _0xae19x46) { return _0xae19xa5 } } } function uGen(_0xae19xa7, _0xae19x49, _0xae19xa8, _0xae19x85, _0xae19xa9) { var _0xae19xaa; _0xae19xaa = Math[_0xebed[17]](_0xae19xa7 / _0xae19xa8); _0xae19xaa = _0xae19x49 * (_0xae19xa7 - (_0xae19xaa * _0xae19xa8)) - (_0xae19xaa * _0xae19x85); return Math[_0xebed[41]]((_0xae19xaa < 0) ? (_0xae19xaa + _0xae19xa9) : _0xae19xaa) } function LEnext() { var _0xae19x2e; this[_0xebed[42]] = uGen(this[_0xebed[42]], 40014, 53668, 12211, 2147483563); this[_0xebed[43]] = uGen(this[_0xebed[43]], 40692, 52774, 3791, 2147483399); _0xae19x2e = Math[_0xebed[17]](this[_0xebed[44]] / 67108862); this[_0xebed[44]] = Math[_0xebed[41]]((this[_0xebed[45]][_0xae19x2e] + this[_0xebed[43]]) % 2147483563); this[_0xebed[45]][_0xae19x2e] = this[_0xebed[42]]; return this[_0xebed[44]] } function LEnint(_0xae19x46) { var _0xae19xa3 = 1; while (_0xae19x46 >= _0xae19xa3) { _0xae19xa3 <<= 1 }; _0xae19xa3--; while (true) { var _0xae19xa5 = this[_0xebed[39]]() & _0xae19xa3; if (_0xae19xa5 <= _0xae19x46) { return _0xae19xa5 } } } function LEcuyer(_0xae19x32) { var _0xae19x2e; this[_0xebed[45]] = new Array(32); this[_0xebed[42]] = this[_0xebed[43]] = (_0xae19x32 & 0x7FFFFFFF); for (_0xae19x2e = 0; _0xae19x2e < 19; _0xae19x2e++) { this[_0xebed[42]] = uGen(this[_0xebed[42]], 40014, 53668, 12211, 2147483563) }; for (_0xae19x2e = 0; _0xae19x2e < 32; _0xae19x2e++) { this[_0xebed[42]] = uGen(this[_0xebed[42]], 40014, 53668, 12211, 2147483563); this[_0xebed[45]][31 - _0xae19x2e] = this[_0xebed[42]] }; this[_0xebed[44]] = this[_0xebed[45]][0]; this[_0xebed[39]] = LEnext; this[_0xebed[26]] = LEnint }