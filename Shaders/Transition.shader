/*
	Treshold-based transition shader
	MMXX ZIK, written for lospec's lowrezjam project
*/
shader_type canvas_item;

uniform float transition_t : hint_range(0, 1) = 1.0; // Transition 'percentage' in [0..1] range
uniform float transition_feather : hint_range(0.001, 1) = 0.001; // Softness of transition
uniform vec4 transition_col : hint_color = vec4(vec3(0.0), 1.0); // Colour of transition
uniform bool transition_invert = false; // Invert the transition?
uniform sampler2D mask_tex; // Sampler texture for transition mask

void fragment ()
{
	// Fetch the screen colour
	vec4 screencol = texture(SCREEN_TEXTURE, SCREEN_UV);
	
	// Fetch the mask texture's red channel as mask value
	// (since we assume the mask texture is grayscale, thus all three rgb values would be same)
	float mask = texture(mask_tex, SCREEN_UV).r;
	
	// Calculate the mixing factor
	// for mixing colour with transition colour based on the mask value
	// (I've used smoothstep to 'feather out' the mixing factor)
	float mixfactor = mix(
		smoothstep(transition_t - transition_feather, transition_t, mask),
		smoothstep(transition_t, transition_t + transition_feather, mask),
		transition_t);

	// Clamp the mixing factor to ensure it is in bounded to [0..1] range for the next step
	mixfactor = clamp(mixfactor, 0.0, 1.0);
	
	// Invert the mixing factor according to transition_invert value
	mixfactor = mix(mixfactor, 1.0 - mixfactor, float(transition_invert));
	
	// Mix the colour with transition colour based on the final calculated mixing factor
	COLOR = mix(transition_col, screencol, mixfactor);
}
