#!/usr/bin/env python3
"""Build PMM_Body_FourArmedWinged out of Big & Small's own body defs.

Source: Big and Small - Framework 1.6
  * SimplyRaces/Defs/Races/FourArms/BodyDef_FourArms.xml   -> the part tree
  * SimplyRaces/Defs/Races/WingedHuman/BodyDef_WingedHuman.xml -> the two BS_Wing parts

The result is B&S's four-armed humanlike with wings added, and the lower pair of
hands moved into two groups of our own (PMM_LowerLeftHand / PMM_LowerRightHand) so
the lower fists stay usable when the upper arms are destroyed. Everything else is
copied verbatim, including B&S's own comments and their attributes.

Run it as `python3 Tools/make_four_armed_winged_body.py [output.xml]`; with no
argument it overwrites the committed def beside it. The source folder is looked up
under the usual Steam locations (the workshop copy first, then local Mods folders)
and printed, because one machine can hold several copies of the framework at once.
Set PMM_BS_RACES to a SimplyRaces/Defs/Races folder to choose one by hand.
"""

import os
import sys
import xml.dom.minidom as md
from pathlib import Path

# Big and Small - Framework: workshop item 2925432336.
BS_WORKSHOP_ID = "2925432336"
BS_RELATIVE = Path("1.6/SimplyRaces/Defs/Races")
BS_NEEDED = ("FourArms/BodyDef_FourArms.xml", "WingedHuman/BodyDef_WingedHuman.xml")
DEFAULT_OUT = Path(__file__).resolve().parent.parent / "Defs/BodyDefs/Body_FourArmedWinged.xml"


def find_bs_races():
    """B&S's Races folder to read: the one PMM_BS_RACES names, or the first found."""
    override = os.environ.get("PMM_BS_RACES")
    if override:
        candidates = [Path(override).expanduser()]
    else:
        candidates = []
        for steamapps in (Path.home() / ".steam/steam/steamapps",
                          Path.home() / ".steam/debian-installation/steamapps",
                          Path.home() / ".local/share/Steam/steamapps"):
            candidates.append(steamapps / "workshop/content/294100" / BS_WORKSHOP_ID / BS_RELATIVE)
            candidates.extend(sorted((steamapps / "common/RimWorld/Mods").glob("*/" + str(BS_RELATIVE))))
    tried = []
    for path in candidates:
        if all((path / name).is_file() for name in BS_NEEDED):
            return path
        tried.append(path)
    raise SystemExit(
        "Big and Small - Framework's bodies were not found. Looked for %s in:\n  %s\n"
        "Set PMM_BS_RACES to the SimplyRaces/Defs/Races folder of the copy you want:\n"
        "  PMM_BS_RACES=~/.steam/steam/steamapps/workshop/content/294100/%s/%s"
        % (BS_NEEDED[0], "\n  ".join(str(path) for path in tried), BS_WORKSHOP_ID, BS_RELATIVE))


HEADER = (
    "Body for the abaddon species - the queen and her soldier daughters alike:\n"
    "\tfour arms and wings.\n"
    "\n"
    "\tGenerated, do not hand-edit without re-running the generator: the part tree is\n"
    "\tBig & Small's own four-armed humanlike (SimplyRaces/.../FourArms/BodyDef_FourArms.xml)\n"
    "\twith the two BS_Wing parts from their winged humanlike inserted, so a race with\n"
    "\tfour arms can still fly. Tools/make_four_armed_winged_body.py in the repo\n"
    "regenerates it after a Big and Small update.\n"
    "\n"
    "\tThe wings keep B&S's parts but not their coverage: theirs is 0.08 each, which\n"
    "\ttheir two-armed body can afford, while four arms already spend 93% of the torso.\n"
    "\t0.02 each keeps every record under the 100% RimWorld warns about in dev mode.\n"
    "\n"
    "\tOne deliberate change: the lower pair of hands keeps its own body part groups\n"
    "\t(PMM_LowerLeftHand / PMM_LowerRightHand, in Defs/BodyPartGroupDefs/) instead of\n"
    "\tsharing vanilla's LeftHand / RightHand. Vanilla decides whether a fist is usable\n"
    "\tby its group, so without this the upper and lower pairs would break together -\n"
    "\twhich is the opposite of what a second pair of arms is for.\n"
    "\n"
    "\tThe extra arms are drawn by nothing: humanlike arms live in the body texture, so\n"
    "\tB&S's own four-armed race shows two arms as well. HANDOFF.md 5.7 holds the art\n"
    "\tspec for the sprite that fixes that."
)


def text_of(parent, tag):
    nodes = parent.getElementsByTagName(tag)
    return nodes[0].firstChild.data.strip() if nodes and nodes[0].firstChild else None


def set_text(parent, tag, value):
    nodes = parent.getElementsByTagName(tag)
    if not nodes or not nodes[0].firstChild:
        raise SystemExit("missing <%s> in %s" % (tag, parent.tagName))
    nodes[0].firstChild.data = value


def ensure_text(doc, parent, tag, value, after_tag):
    """Set <tag>, creating it after <after_tag> when the source def has none."""
    nodes = parent.getElementsByTagName(tag)
    if nodes:
        nodes[0].firstChild.data = value
        return
    node = doc.createElement(tag)
    node.appendChild(doc.createTextNode(value))
    anchor = parent.getElementsByTagName(after_tag)[0]
    parent.insertBefore(node, anchor.nextSibling)


def parts_of(body):
    for core in body.getElementsByTagName("corePart"):
        for parts in core.getElementsByTagName("parts"):
            if parts.parentNode is core:
                return parts
    raise SystemExit("no corePart/parts found")


def strip_whitespace(node):
    for child in list(node.childNodes):
        if child.nodeType == child.TEXT_NODE and not child.data.strip():
            node.removeChild(child)
        else:
            strip_whitespace(child)


def serialize(node, depth, out):
    pad = "\t" * depth
    if node.nodeType == node.COMMENT_NODE:
        for line in node.data.strip().splitlines():
            text = line.strip()
            out.append("%s<!-- %s -->" % (pad, text) if text else "")
        return
    attrs = "".join(' %s="%s"' % (name, value)
                    for name, value in node.attributes.items())
    kids = [c for c in node.childNodes
            if c.nodeType in (c.ELEMENT_NODE, c.COMMENT_NODE)
            or (c.nodeType == c.TEXT_NODE and c.data.strip())]
    if not kids:
        out.append("%s<%s%s/>" % (pad, node.tagName, attrs))
        return
    if len(kids) == 1 and kids[0].nodeType == kids[0].TEXT_NODE:
        out.append("%s<%s%s>%s</%s>" % (pad, node.tagName, attrs,
                                        kids[0].data.strip(), node.tagName))
        return
    out.append("%s<%s%s>" % (pad, node.tagName, attrs))
    for kid in kids:
        serialize(kid, depth + 1, out)
    out.append("%s</%s>" % (pad, node.tagName))


def main():
    if len(sys.argv) > 2:
        raise SystemExit("usage: %s [output.xml]" % Path(sys.argv[0]).name)
    bs = find_bs_races()
    out = Path(sys.argv[1]).expanduser() if len(sys.argv) > 1 else DEFAULT_OUT
    four = md.parse(str(bs / "FourArms/BodyDef_FourArms.xml"))
    winged = md.parse(str(bs / "WingedHuman/BodyDef_WingedHuman.xml"))
    body = four.getElementsByTagName("BodyDef")[0]

    set_text(body, "defName", "PMM_Body_FourArmedWinged")
    set_text(body, "label", "four-armed winged")
    ensure_text(four, body, "description",
                "A four-armed insect woman: two pairs of arms, and a pair of wings on her back.",
                "label")

    # Wings: copy the two BS_Wing parts out of B&S's winged body, untouched.
    wings = []
    for li in winged.getElementsByTagName("li"):
        label = text_of(li, "customLabel")
        if label in ("left wing", "right wing") and text_of(li, "def") == "BS_Wing":
            wings.append(four.importNode(li, True))
    if len(wings) != 2:
        raise SystemExit("expected 2 wing parts, found %d" % len(wings))
    our_parts = parts_of(body)
    for wing in wings:
        our_parts.appendChild(wing)
        # B&S ships each wing with coverage 0.08, which their winged body can afford:
        # it has the vanilla two arms, so its torso children total about 93%. Ours is at
        # 93% BEFORE the wings (four arms), and RimWorld warns once a record's children
        # reach 100%. 0.02 a wing keeps the pair well under the cap and still leaves the
        # wings hittable.
        set_text(wing, "coverage", "0.02")

    # Lower hands: their own groups, so the two pairs fail independently.
    rename = {"LeftHand": "PMM_LowerLeftHand", "RightHand": "PMM_LowerRightHand"}
    moved = 0
    for li in body.getElementsByTagName("li"):
        label = text_of(li, "customLabel")
        if not label or not label.startswith(("left lower", "right lower")):
            continue
        for group in li.getElementsByTagName("li"):
            if group.parentNode.tagName != "groups" or not group.firstChild:
                continue
            name = group.firstChild.data.strip()
            if name in rename:
                group.firstChild.data = rename[name]
                moved += 1

    lines = []
    strip_whitespace(body)
    serialize(body, 1, lines)
    with open(out, "w", encoding="utf-8") as fh:
        fh.write('<?xml version="1.0" encoding="utf-8"?>\n<Defs>\n\n\t<!--\n')
        for line in HEADER.splitlines():
            fh.write("\t\t%s\n" % line.strip() if line.strip() else "\n")
        fh.write("\t-->\n\n" + "\n".join(lines) + "\n\n</Defs>\n")
    print("source: %s" % bs)
    print("wrote:  %s" % out)

    # Verify what we wrote.
    check = md.parse(str(out))
    labels = [text_of(li, "customLabel") for li in check.getElementsByTagName("li")]
    for want in ("left arm", "right arm", "left lower arm", "right lower arm",
                 "left hand", "right hand", "left lower hand", "right lower hand",
                 "left wing", "right wing"):
        if want not in labels:
            raise SystemExit("written body is missing %s" % want)
    groups = [li.firstChild.data.strip()
              for li in check.getElementsByTagName("li")
              if li.parentNode.tagName == "groups" and li.firstChild]
    print("parts: %d labels, %d group entries" % (len(labels), len(groups)))
    print("lower-hand groups moved: %d" % moved)
    print("groups now in use:", sorted(set(groups)))

    # The same rule RimWorld checks in dev mode (BodyDef.ConfigErrors): no record's
    # children may total 100% coverage or more. The wings are what can cross it here,
    # so the check guards the change above rather than trusting the arithmetic.
    worst = 0.0
    worst_owner = None
    for parts in check.getElementsByTagName("parts"):
        total = 0.0
        for child in parts.childNodes:
            if child.nodeType != child.ELEMENT_NODE:
                continue
            coverage = child.getElementsByTagName("coverage")
            if coverage and coverage[0].firstChild:
                total += float(coverage[0].firstChild.data)
        if total > worst:
            worst, worst_owner = total, parts.parentNode.getElementsByTagName("def")[0].firstChild.data
    print("worst coverage sum: %.0f%% (%s)" % (worst * 100, worst_owner))
    if worst >= 1.0:
        raise SystemExit("a record's children exceed 100%% coverage - RimWorld would warn")


if __name__ == "__main__":
    sys.exit(main())
