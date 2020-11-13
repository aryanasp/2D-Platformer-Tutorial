using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseable
{
    // Start is called before the first frame update
    void Use();
    void ExitFromUseArea();
}
