shader_type canvas_item;

uniform vec3 base_color = vec3(0,0,0);
uniform vec4 channel = vec4(1,0,0,0);

void fragment() {
	float alpha = length(texture(TEXTURE, UV) * channel);
	COLOR = vec4(base_color.r, base_color.g, base_color.b, alpha);
}