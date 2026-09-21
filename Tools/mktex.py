#!/usr/bin/env python3
"""Generates the mod's placeholder textures (pure stdlib: zlib + struct).
Each icon is a soft-edged bug silhouette on a transparent background — a body
disc, a smaller head, and two curved antennae. Species vary body colour.
Run from the mod root:  python3 Tools/mktex.py
"""
import math, os, struct, zlib

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

def write_png(path, w, h, px):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    raw = b"".join(b"\x00" + bytes(px[y * w * 4:(y + 1) * w * 4]) for y in range(h))
    def chunk(tag, data):
        c = tag + data
        return struct.pack(">I", len(data)) + c + struct.pack(">I", zlib.crc32(c) & 0xffffffff)
    png = (b"\x89PNG\r\n\x1a\n"
           + chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0))
           + chunk(b"IDAT", zlib.compress(raw, 9))
           + chunk(b"IEND", b""))
    with open(path, "wb") as f:
        f.write(png)

def bug_icon(size, body_col, belly_col):
    """A simple top-down bug: abdomen disc, head disc, two antennae curves."""
    px = bytearray(size * size * 4)

    def blend(x, y, r, g, b, a):
        a = max(0, min(255, int(a)))
        if 0 <= x < size and 0 <= y < size and a > 0:
            i = (y * size + x) * 4
            na = a + px[i + 3] * (255 - a) // 255
            if na == 0:
                return
            for c, v in ((0, r), (1, g), (2, b)):
                px[i + c] = max(0, min(255, (v * a + px[i + c] * px[i + 3] * (255 - a) // 255) // na))
            px[i + 3] = na

    def disc(cx, cy, rad, col, soft=1.5):
        r, g, b, a = col
        for y in range(int(cy - rad - 2), int(cy + rad + 3)):
            for x in range(int(cx - rad - 2), int(cx + rad + 3)):
                d = math.hypot(x - cx + .5, y - cy + .5)
                if d <= rad:
                    blend(x, y, r, g, b, a)
                elif d <= rad + soft:
                    blend(x, y, r, g, b, int(a * (rad + soft - d) / soft))

    s = size / 64.0  # design on a 64px grid, scale to size
    cx = size / 2
    # antennae (two arcs from the head towards the top corners)
    for t in range(0, 60):
        a = t / 59.0
        ang = -math.pi / 2 + (a - 0.5) * 1.5
        rr = 22 * s
        for sgn in (-1, 1):
            x = int(cx + sgn * math.cos(ang) * rr + sgn * 4 * s * a)
            y = int(26 * s - math.sin(ang) * rr - 6 * s * a)
            blend(x, y, *body_col[:3], int(body_col[3] * (1 - a * 0.5)))
            blend(x + sgn, y, *body_col[:3], int(body_col[3] * 0.5))
    # abdomen (big), then head (small, on top)
    disc(cx, 40 * s, 17 * s, body_col)
    disc(cx, 40 * s, 12 * s, belly_col)
    disc(cx, 24 * s, 9 * s, body_col)
    return px

def egg_sac(size, sac_col, glow_col):
    """A squat sac under a soft glow: a wide low body, a pale sheen and dark speckles.
    Same pixel helpers as bug_icon (kept local so each generator stands alone)."""
    px = bytearray(size * size * 4)

    def blend(x, y, r, g, b, a):
        a = max(0, min(255, int(a)))
        if 0 <= x < size and 0 <= y < size and a > 0:
            i = (y * size + x) * 4
            na = a + px[i + 3] * (255 - a) // 255
            if na == 0:
                return
            for c, v in ((0, r), (1, g), (2, b)):
                px[i + c] = max(0, min(255, (v * a + px[i + c] * px[i + 3] * (255 - a) // 255) // na))
            px[i + 3] = na

    def disc(cx, cy, rad, col, soft=1.5):
        r, g, b, a = col
        for y in range(int(cy - rad - 2), int(cy + rad + 3)):
            for x in range(int(cx - rad - 2), int(cx + rad + 3)):
                d = math.hypot(x - cx + .5, y - cy + .5)
                if d <= rad:
                    blend(x, y, r, g, b, a)
                elif d <= rad + soft:
                    blend(x, y, r, g, b, int(a * (rad + soft - d) / soft))

    s = size / 64.0
    cx = size / 2
    glow = (glow_col[0], glow_col[1], glow_col[2], 45)
    sheen = (min(255, sac_col[0] + 40), min(255, sac_col[1] + 45), min(255, sac_col[2] + 30), 210)
    disc(cx, 36 * s, 23 * s, glow, soft=9)          # bioluminescent halo
    disc(cx, 36 * s, 19 * s, sac_col)               # the sac itself
    disc(cx, 30 * s, 11 * s, sheen)                 # wet sheen on top
    for k in range(14):                             # speckles around the base
        a = k / 14.0 * math.tau
        disc(cx + math.cos(a) * 13 * s, 38 * s + math.sin(a) * 10 * s, 1.6 * s, (30, 45, 25, 170))
    return px

SPECIES = {
    # name: (body colour, belly colour)
    "PMM_DevilBug":     ((90, 70, 40, 255), (150, 120, 70, 255)),   # brown carapace
    "PMM_GiantAnt":     ((120, 60, 40, 255), (180, 100, 60, 255)),  # reddish ant
    "PMM_SoldierBeetle":((60, 60, 70, 255), (110, 110, 130, 255)),  # steel carapace
    "PMM_Greenworm":    ((90, 140, 60, 255), (150, 190, 100, 255)), # caterpillar green
    "PMM_VampMosquito": ((70, 50, 80, 255), (130, 90, 140, 255)),   # dusky purple
    "PMM_Abaddon":      ((40, 40, 45, 255), (90, 80, 100, 255)),    # near-black queen
}

for name, (body, belly) in SPECIES.items():
    write_png(os.path.join(ROOT, "Textures", "UI", "Icons", "Xenotypes", name + ".png"),
              128, 128, bug_icon(128, body, belly))
    print("wrote", name)

# Shared insect gene icon: a single feeler-silhouette
write_png(os.path.join(ROOT, "Textures", "UI", "Icons", "Genes", "Gene_Insect.png"),
          128, 128, bug_icon(128, (70, 90, 50, 255), (120, 150, 80, 255)))
print("wrote Gene_Insect")

# Phase 5, the brood: the sac the abaddon throws, its projectile, and the ability gizmo.
SAC, GLOW = (120, 150, 90, 255), (150, 200, 120, 255)
for path, size in (("Things/Building/PMM_EggSac", 64),
                   ("Things/Projectile/PMM_EggSac", 32),
                   ("UI/Icons/Abilities/PMM_EggSpew", 128)):
    write_png(os.path.join(ROOT, "Textures", *path.split("/")) + ".png",
              size, size, egg_sac(size, SAC, GLOW))
    print("wrote", path)
