# Prompty i źródła

Grafikę utworzono wbudowanym narzędziem ImageGen. Następnie wykonano techniczny eksport do siatki 32×32: jednolita skala, nearest-neighbor, binarna przezroczystość i indeksowana paleta 16 kolorów.

## Generowanie

Use case: stylized-concept.
Create a production pixel-art animation SPRITE ATLAS for a top-down 2D game in the perspective of Stardew Valley. Original fierce GREY WOLF, quadruped animal, NOT a humanoid or werewolf. Raised hackles, pointed ears, long snout, fluffy tail, dark charcoal back, medium neutral grey body, pale grey muzzle and paws, tiny amber eyes, small white visible fangs. Limited 12-color pixel palette, clearly stepped pixel edges, compact readable 32x32-pixel sprite design. Camera is fixed slightly overhead three-quarter top-down, all four cardinal directions.

OUTPUT: a single transparent PNG, EXACTLY 1024 pixels wide by 1280 pixels high. It represents a 256x320 native pixel atlas enlarged exactly 4x with nearest-neighbor; each art pixel is exactly a solid 4x4 output block. NO anti-aliasing, NO gradients, NO tiny high-resolution details.
LAYOUT: EXACTLY 8 equally spaced columns and 10 equally spaced rows, 80 sprites total. EVERY cell is 128x128 output pixels (32x32 native). NO page margins, NO gaps between cells, NO headers, NO words, NO labels, NO grid lines, NO frames or decorations. Transparent empty pixels around each wolf within each cell. Never draw a checkerboard background. No ground shadows, no scene props. Draw exactly ONE wolf wholly inside EVERY cell.
Each wolf fits within native x=2..29,y=2..29 and is registered to the same central root point in every cell. Side views about 28x21 native pixels including tail, front/back views about 19x27 native pixels. Consistent body volume and anatomy across all 80 cells. Four frames play LEFT TO RIGHT in groups of 4 cells. The second group starts at column 5. Treat every four-cell group as one independent animation; no turning between directions.

EXACT ROW CONTENT, COUNT ROWS FROM TOP:
Row 1: IDLE DOWN toward viewer, frames 1,2,3,4; then IDLE UP away from viewer, frames 1,2,3,4.
Row 2: IDLE LEFT, frames 1,2,3,4; then IDLE RIGHT, frames 1,2,3,4.
Row 3: RUN DOWN, frames 1,2,3,4; then RUN UP, frames 1,2,3,4.
Row 4: RUN LEFT, frames 1,2,3,4; then RUN RIGHT, frames 1,2,3,4.
Row 5: SLEEP DOWN, frames 1,2,3,4; then SLEEP UP, frames 1,2,3,4.
Row 6: SLEEP LEFT, frames 1,2,3,4; then SLEEP RIGHT, frames 1,2,3,4.
Row 7: ATTACK DOWN, frames 1,2,3,4; then ATTACK UP, frames 1,2,3,4.
Row 8: ATTACK LEFT, frames 1,2,3,4; then ATTACK RIGHT, frames 1,2,3,4.
Row 9: DEATH DOWN, frames 1,2,3,4; then DEATH UP, frames 1,2,3,4.
Row 10: DEATH LEFT, frames 1,2,3,4; then DEATH RIGHT, frames 1,2,3,4.

ANIMATION CHOREOGRAPHY:
IDLE: standing tense, subtle breathing chest down/up, tiny ear/tail movement, feet planted. Frames loop.
RUN: convincing fast quadruped gallop, four clearly distinct poses: rear push/front reach, extended airborne, forepaw contact/hind tucked, compact gathered airborne. Forward direction fixed, remain in place in cells. Frames loop.
SLEEP: curled up on ground throughout all four frames, head resting on forepaws, closed eyes, tail wrapped around body, subtle breathing. All four directions remain visually distinguishable by head placement. No Z letters. Frames loop.
ATTACK: 1 low crouched anticipation, 2 forward lunge with mouth opening, 3 extended snapping bite with bared fangs and front paws reaching, 4 recover to angry standing stance. No slash effects or weapons.
DEATH: 1 standing hurt with ears back, 2 knees buckling, 3 falling onto side, 4 fully collapsed motionless wolf on side with eyes closed. No gore, no blood. Last frame stays, not a loop. Keep the head pointing toward the specified direction.

Prioritize correct uniform GRID, the exact direction and animation in each row, and genuinely low resolution pixel art. No extra wolves, no missing cells, no text anywhere.

## Korekta stylu

Use case: style-transfer.
Edit the provided wolf sprite atlas. Preserve the EXACT 8 columns x 10 rows layout (80 wolves), all wolf identities, neutral grey fur, each direction in its cell, each animation row assignment, and the transparent alpha background. This is a strict pixel-art cleanup pass for native 32x32 GAME SPRITES.
CRITICAL CHANGE: redraw EVERY wolf using chunky, flat, deliberately placed SQUARE PIXEL CLUSTERS on a strict 32x32 logical pixel grid per cell. Each wolf itself only 26-28 logical pixels wide/high maximum. Visibly chunky 1-pixel dark outline and flat cel-shaded pixel clusters, 12-16 color palette maximum. NO fine fur, NO smooth shading, NO anti-aliasing, NO thin high-resolution contours. The supplied image currently has far too many tiny details; SIMPLIFY radically to genuine retro SNES pixel art like a Stardew Valley animal. Output preview at an exact integer enlargement with each logical pixel becoming a 4x4 solid square. Native atlas 256x320; desired preview 1024x1280. All pixel blocks same size.
Keep transparent gutters within each tile and all silhouettes contained within tiles. NO labels, NO gridlines, NO drawn checkerboard, NO shadows.
The 8 columns contain two four-frame sequences per row. Rows 1,3,5,7,9: first four face DOWN, second four face UP. Rows 2,4,6,8,10: first four face LEFT, second four face RIGHT.
Rows 1-2 idle: tense angry standing, fangs, tiny amber eyes, breathe.
Rows 3-4 run: exaggerate the FOUR DISTINCT quadruped galloping limb configurations, ESPECIALLY DOWN and UP on row 3: front legs extended, gathered, planted, alternate reach; hind legs cycle too. Keep same body size and direction, animation in place.
Rows 5-6 sleep: every wolf has CLOSED eyes as small dark horizontal dashes; body curled and head resting throughout, very subtle breathing.
Rows 7-8 attack: low anticipation, forward leap mouth open, snapping bite, return. UP attack shows back of head, NOT a disembodied mouth above head.
Rows 9-10 death: standing hurt, legs buckling, falling, lying motionless eyes shut. Preserve direction of head in collapsed poses: down toward bottom, up toward top, left toward left, right toward right.
All 80 cells remain present in a regular 8x10 uniform grid. No empty cells. Preserve transparency.

## Przezroczystość

Use case: background-extraction.
Input image is a finished pixel-art wolf sprite atlas with 80 sprites in an 8 column by 10 row grid.
ONLY REMOVE THE BACKGROUND. The light grey and white checkerboard is erroneously painted into the image. REMOVE EVERY CHECKERBOARD PIXEL, including between legs and in all empty parts of each cell, and replace it with genuine PNG alpha transparency.
Preserve ALL 80 wolves, all their existing pixel art, exact sizes, exact positions, colors, hard edges and 8x10 cell layout. Do NOT redraw, restyle, move, crop or resize any wolf. Do not add anything. Preserve original full canvas.
The output must be an RGBA PNG with truly TRANSPARENT background (alpha 0), NOT an RGB image of a checkerboard. The wolf pixels are opaque. Do not paint a checkerboard, no solid background, no shadows. This request is solely to remove the fake checkerboard background.

