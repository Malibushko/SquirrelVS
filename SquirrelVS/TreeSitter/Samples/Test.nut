function bar(a, b, c)
{
    local b = 5;

    for (local i = 5; i < 10; i++)
    {
        b *= 5;
    }

    return b * 2;
}

function Hello()
{
    bar((1, 2, 3));
}

function Hello()
{
}