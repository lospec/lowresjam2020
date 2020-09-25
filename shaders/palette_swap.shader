shader_type canvas_item;

// Hardcoded for palettes of width 32 and below due to web build limitations
// regarding dynamic loops

// Credit to https://stackoverflow.com/a/9018153/8004215
// for the color distance algorithm

uniform bool enabled;
uniform sampler2D palette_tex; // 1-pixel tall texture containing the palette
uniform float palette_transition : hint_range(0, 1) = 1.0;

void fragment() {
	if (!enabled) {
		return;
	}
	
	// Get existing color
	vec4 col = texture(SCREEN_TEXTURE, SCREEN_UV);
	
	// Iterate through palette colors and choose best fit
	vec2 palette_pixel_size = 1.0 / vec2(32.0, 1.0);
	
	float current_best_closeness;
	vec4 closest_palette_col;
	for (int i = 0; i < 32; i++) {
		vec4 palette_col = texture(palette_tex, palette_pixel_size * float(i));
		float closeness_distance = sqrt(
			pow(palette_col.r - col.r, 2) +
			pow(palette_col.g - col.g, 2) +
			pow(palette_col.b - col.b, 2));
		
		float closeness = (1.0 - closeness_distance/sqrt(
			pow(255, 2) +
			pow(255, 2) +
			pow(255, 2)));
		
		if (closeness > current_best_closeness) {
			current_best_closeness = closeness;
			closest_palette_col = palette_col;
		}
	}
	
	COLOR = mix(col, closest_palette_col, palette_transition);
}