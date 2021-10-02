using UnityEngine;

public abstract class FixedFrameInterpolator<T> {
    private T currentValue;
    private T previousValue;

    public T Value => currentValue;
    public T Interpolated => Lerp(previousValue, currentValue, Mathf.Clamp01((Time.time - Time.fixedTime) / Time.fixedDeltaTime));

    public static implicit operator T(FixedFrameInterpolator<T> interpolator) => interpolator.Value;

    public abstract T Lerp(T a, T b, float t);

    public void SetValue(T newValue) {
        previousValue = currentValue;
        currentValue = newValue;
    }
}

public class FixedFrameInterpolatorFloat : FixedFrameInterpolator<float> {
    public override float Lerp(float a, float b, float t) {
        return Mathf.Lerp(a, b, t);
    }
}

public class FixedFrameInterpolatorVector3 : FixedFrameInterpolator<Vector3> {
    public override Vector3 Lerp(Vector3 a, Vector3 b, float t) {
        return Vector3.Lerp(a, b, t);
    }
}

public class FixedFrameInterpolatorQuaternion : FixedFrameInterpolator<Quaternion> {
    public override Quaternion Lerp(Quaternion a, Quaternion b, float t) {
        return Quaternion.Slerp(a, b, t);
    }
}
