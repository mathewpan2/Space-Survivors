using System;

public interface IProjectile
{
    public void Hit(IHealth target);

    public void moveTick();
}