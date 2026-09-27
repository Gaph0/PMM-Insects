#!/usr/bin/env python3
"""Generates the placeholder sprite set for the abaddon folk's LOWER pair of arms.

These are placeholders, not finished art. They exist so the render node on
PMM_RaceTracker_AbaddonFolk has something to draw and no texture error appears;
replace the PNGs (same names, same 512x512 canvas) with real art when it exists.

Draw them in near-white: the render node uses colorType Skin, so the game
multiplies the sprite by the pawn's chitin tone.

Six files, the set Big & Small's own tail uses - south, north and east, each with
an "_m" mirror - because pawns render east and west from the same sprite:

    Textures/RaceDefaults/PMM_AbaddonFolk/PMM_LowerArms_south.png
    .../PMM_LowerArms_southm.png
    .../PMM_LowerArms_north.png
    .../PMM_LowerArms_northm.png
    .../PMM_LowerArms_east.png
    .../PMM_LowerArms_eastm.png

Run from the mod root:  python3 Tools/mktex_arms.py
"""

import math
import os
import struct
import zlib

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "Textures", "RaceDefaults", "PMM_AbaddonFolk")
SIZE = 512


def write_png(path, w, h, px):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    raw = b"".join(b"\x00" + bytes(px[y * w * 4:(y + 1) * w * 4]) for y in range(h))

    def chunk(tag, data):
        blob = tag + data
        return struct.pack(">I", len(data)) + blob + struct.pack(">I", zlib.crc32(blob) & 0xffffffff)

    png = (b"\x89PNG\r\n\x1a\n"
           + chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0))
           + chunk(b"IDAT", zlib.compress(raw, 9))
           + chunk(b"IEND", b""))
    with open(path, "wb") as fh:
        fh.write(png)


class Canvas:
    """Alpha-blended greyscale drawing, then optional horizontal mirror."""

    def __init__(self, size):
        self.size = size
        self.px = bytearray(size * size * 4)

    def blend(self, x, y, shade, alpha):
        x = int(x)
        y = int(y)
        alpha = max(0, min(255, int(alpha)))
        if not (0 <= x < self.size and 0 <= y < self.size) or alpha == 0:
            return
        i = (y * self.size + x) * 4
        old = self.px[i + 3]
        new_a = alpha + old * (255 - alpha) // 255
        if new_a == 0:
            return
        value = (shade * alpha + self.px[i] * old * (255 - alpha) // 255) // new_a
        self.px[i] = self.px[i + 1] = self.px[i + 2] = max(0, min(255, value))
        self.px[i + 3] = new_a

    def limb(self, points, radius, shade):
        """A tapered capsule chain through `points`, with a soft 2px edge."""
        for step in range(len(points) - 1):
            (x0, y0), (x1, y1) = points[step], points[step + 1]
            r0 = radius[step]
            r1 = radius[step + 1]
            length = math.hypot(x1 - x0, y1 - y0) or 1
            for t in range(int(length) + 1):
                f = t / length
                cx = x0 + (x1 - x0) * f
                cy = y0 + (y1 - y0) * f
                rad = r0 + (r1 - r0) * f
                for dy in range(int(-rad - 2), int(rad + 3)):
                    for dx in range(int(-rad - 2), int(rad + 3)):
                        d = math.hypot(dx, dy)
                        if d <= rad:
                            self.blend(cx + dx, cy + dy, shade, 255)
                        elif d <= rad + 2:
                            self.blend(cx + dx, cy + dy, shade, int(255 * (rad + 2 - d) / 2))

    def hand(self, cx, cy, radius):
        for dy in range(int(-radius - 2), int(radius + 3)):
            for dx in range(int(-radius - 2), int(radius + 3)):
                d = math.hypot(dx, dy)
                if d <= radius:
                    self.blend(cx + dx, cy + dy, 250, 255)
                elif d <= radius + 2:
                    self.blend(cx + dx, cy + dy, 250, int(255 * (radius + 2 - d) / 2))

    def mirrored(self):
        other = Canvas(self.size)
        for y in range(self.size):
            row = y * self.size * 4
            for x in range(self.size):
                i = row + x * 4
                j = row + (self.size - 1 - x) * 4
                other.px[j:j + 4] = self.px[i:i + 4]
        return other

    def save(self, name):
        write_png(os.path.join(OUT, name), self.size, self.size, self.px)


def facing_arms():
    """Front (and back): both lower arms hanging out and down from the waist."""
    c = Canvas(SIZE)
    for side in (-1, 1):
        shoulder = (256 + side * 34, 168)
        elbow = (256 + side * 96, 232)
        wrist = (256 + side * 112, 296)
        c.limb([shoulder, elbow], (17, 14), 232)
        c.limb([elbow, wrist], (14, 11), 240)
        c.hand(wrist[0] + side * 6, wrist[1] + 12, 13)
    return c


def side_arms():
    """East: one arm forward of the hip, one behind it."""
    c = Canvas(SIZE)
    c.limb([(248, 176), (318, 226)], (16, 13), 236)
    c.limb([(318, 226), (326, 288)], (13, 10), 244)
    c.hand(330, 300, 12)
    c.limb([(242, 182), (206, 236)], (15, 12), 224)
    c.limb([(206, 236), (196, 288)], (12, 9), 232)
    c.hand(192, 298, 11)
    return c


def main():
    front = facing_arms()
    front.save("PMM_LowerArms_south.png")
    front.save("PMM_LowerArms_north.png")
    front.mirrored().save("PMM_LowerArms_southm.png")
    front.mirrored().save("PMM_LowerArms_northm.png")
    side = side_arms()
    side.save("PMM_LowerArms_east.png")
    side.mirrored().save("PMM_LowerArms_eastm.png")
    print("wrote 6 placeholder sprites to %s" % OUT)


if __name__ == "__main__":
    main()
