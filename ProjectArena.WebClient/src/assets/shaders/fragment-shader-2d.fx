precision mediump float;

uniform sampler2D u_color;
uniform sampler2D u_backgroundColor;
uniform sampler2D u_texture;

varying vec2 v_texCoord;
varying vec2 v_position;

void main() {
   vec4 color = texture2D(u_color, v_position);
   vec3 backgroundColor = texture2D(u_backgroundColor, v_position).rgb;
   vec4 textureColor = texture2D(u_texture, v_texCoord);
   float alpha = color.a * textureColor.a * 255.0;
   gl_FragColor = vec4(
       color.r * textureColor.r * alpha + backgroundColor.r * (1.0 - alpha),
       color.g * textureColor.r * alpha + backgroundColor.g * (1.0 - alpha),
       color.b * textureColor.r * alpha + backgroundColor.b * (1.0 - alpha),
       1.0);
}