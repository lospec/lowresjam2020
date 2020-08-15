/*
	Treshold-based transition shader
	MMXX ZIK, written for lospec's lowrezjam project
*/
shader_type canvas_item;

uniform float transition_time : hint_range(0, 1) = 1.0; // Transition 'percentage'/time in [0..1] range
uniform float transition_feather : hint_range(0.001, 1) = 0.001; // Softness of transition
uniform vec4 transition_col : hint_color = vec4(vec3(0.0), 1.0); // Colour of transition
uniform bool transition_invert_mask = false; // Invert the transition mask?
uniform bool transition_invert_time = false; // Invert the transition percentage/time?
uniform bool transition_flip_x 		= false; // Flip the transition mask on x axis?
uniform bool transition_flip_y 		= false; // Flip the transition mask on y axis?
uniform bool transition_dither 		= true; // Use dithering to ensure that the transition is either opaque or transparent?
uniform sampler2D mask_tex; // Sampler texture for transition mask
uniform sampler2D dither_tex; // Sampler texture for dithering mask

void fragment ()
{
	
	float final_t = transition_time;
	vec2 final_uv = SCREEN_UV;
	
	// Calculate variables from parameters
	// (transition mask : flip x/y)
	if (transition_flip_x)
		final_uv.x = 1.0 - final_uv.x;
	if (transition_flip_y)
		final_uv.y = 1.0 - final_uv.y;
	// (time : invert time)
	if (transition_invert_time)
		final_t = 1.0 - final_t;
	
	// Fetch the screen colour
	vec4 screencol = texture(SCREEN_TEXTURE, SCREEN_UV);
	
	// Fetch the dither texture
	float dither = texture(dither_tex, SCREEN_UV).r;
	
	// Fetch the mask texture's red channel as mask value
	// (since we assume the mask texture is grayscale, thus all three rgb values would be same)
	float mask = texture(mask_tex, final_uv).r;
	
	// Calculate the mixing factor
	// for mixing colour with transition colour based on the mask value
	// (I've used smoothstep to 'feather out' the mixing factor)
	float mixfactor = mix(
		smoothstep(final_t - transition_feather, final_t, mask),
		smoothstep(final_t, final_t + transition_feather, mask),
		final_t);

	// Clamp the mixing factor to ensure it is in bounded to [0..1] range for the next step
	mixfactor = clamp(mixfactor, 0.0, 1.0);
	
	// Invert the mixing factor according to transition_invert value
	// this effectively inverts the mask : the black bits would become white & vice versa
	mixfactor = mix(mixfactor, 1.0 - mixfactor, float(transition_invert_mask));
	
	if (transition_dither)
	{
		const bool QUANTIZE_DITHER = true; // Quantize the dither or not?
		float final_mixfactor = 1.0;
		
		// Dither according to calculated mix factor and dither pattern
		if (QUANTIZE_DITHER) // Quantize : round up to nearest integer, that means it's either 0 or 1
		{
			// final_mixfactor = clamp(floor(max(dither - final_t, 0.0) + 0.5), 0.0, 1.0);
			// Welp, damn. I've tried to use the commented code above to do the same thing as the code below (but in an elegant way)
			// but there was one edge case that couldn't be solved using the above code... So I'm using this branching code :/
			if (mixfactor < dither || mixfactor == 0.0)
				final_mixfactor = 0.0;
			else
				final_mixfactor = 1.0;
		}
		else // Don't quanitze : transform the dither value into -0.5..0.5 range and add it to the mixing factor
		{
			// this one still has an artefact that couldn't be fixed
			final_mixfactor = clamp(mixfactor + (dither - 0.5), 0.0, 1.0);
		}
		
		COLOR = mix(transition_col, screencol, final_mixfactor);
	}
	else
	{
		// Mix the colour with transition colour based on the final calculated mixing factor (without dithering)
		COLOR = mix(transition_col, screencol, mixfactor);
	}
}
