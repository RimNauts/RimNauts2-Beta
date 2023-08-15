using System;
using UnityEngine;

namespace RimNauts2.World.Objects {
    public class Ship : NEO {
        Vector3? last_position = null;

        public Ship(
            string texture_path = null,
            Vector3? orbit_position = null,
            float? orbit_speed = null,
            Vector3? draw_size = null,
            int? period = null,
            int? time_offset = null,
            int? orbit_direction = null,
            float? color = null,
            float? rotation_angle = null,
            float? transformation_rotation_angle = null,
            Vector3? current_position = null,
            Vector3? target_position = null
        ) : base(
            Type.Ship,
            texture_path,
            orbit_position,
            orbit_speed,
            draw_size,
            period,
            time_offset,
            orbit_direction,
            color,
            rotation_angle,
            transformation_rotation_angle,
            current_position,
            target_position
        ) { }

        public override void randomize() { }

        public override void update() {
            base.update();
            if (object_holder == null) return;
            if (!object_holder.Targeter.valid_target()) {
                target_position = Vector3.zero;
                return;
            }
            target_position = object_holder.Targeter.get_coords();
        }

        public override void update_when_unpaused() {
            base.update_when_unpaused();
        }

        public override void update_when_camera_moved() {
            base.update_when_camera_moved();
            if (last_position == null) return;
            update_rotation((Vector3) last_position, current_position);
        }

        public override void update_position(int tick) {
            if (target_position == Vector3.zero) return;
            last_position = get_position();
            current_position = Vector3.Slerp(get_position(), target_position, 0.01f * orbit_speed);
            update_rotation((Vector3) last_position, current_position);
        }

        public void update_rotation(Vector3 current_position, Vector3 target_position) {
            //Vector3 direction = target_position - current_position;
            Vector3 direction = Vector3.Cross(target_position - current_position, RenderingManager.center);
            Vector3 direction_normalized = direction.normalized;
            float yaw_angle = (float) (Math.Atan2(direction_normalized.x, direction_normalized.z) * (180 / Math.PI));
            Vector3 axis = Vector3.up;

            //extra_rotation = Quaternion.LookRotation(direction);
            extra_rotation = Quaternion.AngleAxis(yaw_angle, axis);

            Vector3 center = RenderingManager.center;
            center.x = Math.Abs(center.x);
            center.y = Math.Abs(center.y);
            center.z = Math.Abs(center.z);
            if (center.x >= center.y && center.x >= center.z) {
                extra_rotation.y = 0;
                extra_rotation.z = 0;
            } else if (center.y >= center.x && center.y >= center.z) {
                extra_rotation.x = 0;
                extra_rotation.z = 0;
            } else if (center.z >= center.x && center.z >= center.y) {
                extra_rotation.x = 0;
                extra_rotation.y = 0;
            }
        }
    }
}
